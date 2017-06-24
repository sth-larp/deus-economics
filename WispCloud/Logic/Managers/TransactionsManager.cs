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
        private ConstantManager _constantManager;

        public TransactionsManager(UserContext context) : base(context)
        {
            _userManager = new UserManager(UserContext);
            _rightsManager = new RightsManager(UserContext);
            _constantManager = new ConstantManager(UserContext);
        }

        public void Transfer(TransferClientData data)
        {
            var receiverAcc = _userManager.FindById(data.Receiver);
            Try.NotNull(receiverAcc, $"Cant find account with login: {data.Receiver}.");

            var senderAcc = _userManager.FindById(data.Sender);
            Try.NotNull(senderAcc, $"Cant find account with login: {data.Sender}.");

            using (var dbTransact = UserContext.Data.Database.BeginTransaction())
            {
                UserContext.Data.BeginFastSave();

                _rightsManager.CheckForAccessOverSlave(senderAcc, AccountAccessRoles.Withdraw);

                Try.Condition(senderAcc.Cash >= data.Amount, $"Not enought money");
                Try.Condition(data.Amount > 0, $"Can't transfer negative or zero funds");

                var transaction = new Transaction(senderAcc, receiverAcc, data.Amount);
                transaction.Type = TransactionType.Normal;
                transaction.Comment = data.Description ?? "";

                var taxedTransactions = _constantManager.TakeTax(transaction);
                taxedTransactions.ForEach(x =>
                {
                    x.SenderAccount.Cash -= data.Amount;
                    x.ReceiverAccount.Cash += data.Amount;

                    UserContext.Accounts.Update(x.SenderAccount);
                    UserContext.Accounts.Update(x.ReceiverAccount);

                    UserContext.Data.Transactions.Add(x);

                    UserContext.Data.SaveChanges();
                });

                dbTransact.Commit();
            }
        }

        public List<Transaction> GetHistory(string login, int take, int skip)
        {
            Try.NotEmpty(login, $"Поле {nameof(login)} не должно быть пустым");
            Try.Condition(skip >= 0, $"Поле {nameof(skip)} не должно быть отрицательным");
            Try.Condition(take >= 0, $"Поле {nameof(take)} не должно быть отрицательным");

            _rightsManager.CheckForAccessOverSlave(login, AccountAccessRoles.Read);

            var ret = UserContext.Data.Transactions
                .Where(x => x.Receiver == login || x.Sender == login)
                .OrderByDescending(x => x.Time)
                .Skip(skip).Take(take);
            return ret.ToList();
        }
    }
}