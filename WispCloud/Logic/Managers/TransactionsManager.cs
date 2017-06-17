using System.Collections.Generic;
using System.Linq;
using DeusCloud.Data.Entities.Access;
using DeusCloud.Data.Entities.Transactions;
using DeusCloud.Exceptions;
using DeusCloud.Identity;
using DeusCloud.Logic.Client;
using DeusCloud.Logic.CommonBase;
using Microsoft.AspNet.Identity;

namespace DeusCloud.Logic.Managers
{
    public sealed class TransactionsManager : ContextHolder
    {
        private UserManager _userManager;
        private RightsManager _rightsManager;
        private TaxManager _taxManager;

        public TransactionsManager(UserContext context) : base(context)
        {
            _userManager = new UserManager(UserContext);
            _rightsManager = new RightsManager(UserContext);
            _taxManager = new TaxManager(UserContext);
        }

        public void Transfer(string sender, string receiver, float amount)
        {
            _rightsManager.CheckCurrentUserActive();

            var receiverAcc = _userManager.FindById(receiver);
            Try.NotNull(receiverAcc, $"Cant find account with login: {receiver}.");

            var senderAcc = _userManager.FindById(sender);
            Try.NotNull(senderAcc, $"Cant find account with login: {sender}.");

            using (var dbTransact = UserContext.Data.Database.BeginTransaction())
            {
                UserContext.Data.BeginFastSave();

                _rightsManager.CheckForAccessOverSlave(sender, AccountAccessRoles.Withdraw);

                Try.Condition(senderAcc.Cash >= amount, $"Not enought money");
                Try.Condition(amount > 0, $"Can't transfer negative or zero funds");

                var transaction = new Transaction(senderAcc, receiverAcc, amount);
                transaction.Type = TransactionType.Normal;
                transaction.Comment = "Простая транзакция";

                var taxedTransactions = _taxManager.TakeTax(transaction);
                taxedTransactions.ForEach(x =>
                {
                    x.SenderAccount.Cash -= amount;
                    x.ReceiverAccount.Cash += amount;

                    UserContext.Accounts.Update(x.SenderAccount);
                    UserContext.Accounts.Update(x.ReceiverAccount);

                    UserContext.Data.Transactions.Add(x);

                    UserContext.Data.SaveChanges();
                });

                dbTransact.Commit();
            }
        }

        public List<Transaction> GetHistory(string login)
        {
            _rightsManager.CheckForAccessOverSlave(login, AccountAccessRoles.Read);
            var ret = UserContext.Data.Transactions.Where(
                x => x.Receiver == login || x.Sender == login);
            return ret.ToList();
        }
    }
}