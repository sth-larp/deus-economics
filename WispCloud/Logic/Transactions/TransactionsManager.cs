using System.Collections.Generic;
using System.Linq;
using DeusCloud.Data.Entities;
using DeusCloud.Data.Entities.Access;
using DeusCloud.Data.Entities.Transactions;
using DeusCloud.Exceptions;
using DeusCloud.Identity;
using DeusCloud.Logic.CommonBase;
using DeusCloud.Logic.Events.Client;
using DeusCloud.Logic.Rights;
using Microsoft.AspNet.Identity;

namespace DeusCloud.Logic.Transactions
{
    public sealed class TransactionsManager : ContextHolder
    {
        private UserManager _userManager;
        private RightsManager _rightsManager;

        public TransactionsManager(UserContext context): base(context)
        {
            _userManager = new UserManager(UserContext);
            _rightsManager = new RightsManager(UserContext);
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

                senderAcc.Cash -= amount;
                receiverAcc.Cash += amount;

                UserContext.Accounts.Update(senderAcc);
                UserContext.Accounts.Update(receiverAcc);

                var transaction = new Transaction(senderAcc, receiverAcc, amount);
                transaction.Type = TransactionType.Normal;
                UserContext.Data.Transactions.Add(transaction);

                UserContext.Data.SaveChanges();
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