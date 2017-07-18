using System;
using System.Collections.Generic;
using System.Linq;
using DeusCloud.Data.Entities.Access;
using DeusCloud.Data.Entities.Accounts;
using DeusCloud.Data.Entities.Transactions;
using DeusCloud.Exceptions;
using DeusCloud.Identity;
using DeusCloud.Logic.Client;
using DeusCloud.Logic.CommonBase;
using DeusCloud.Logic.Server;
using Microsoft.AspNet.Identity;

namespace DeusCloud.Logic.Managers
{
    public sealed class TransactionsManager : ContextHolder
    {
        private UserManager _userManager;
        private RightsManager _rightsManager;
        private ConstantManager _constantManager;
        private InsuranceManager _insuranceManager;

        public TransactionsManager(UserContext context) : base(context)
        {
            _userManager = new UserManager(UserContext);
            _rightsManager = new RightsManager(UserContext);
            _constantManager = new ConstantManager(UserContext);
            _insuranceManager = new InsuranceManager(UserContext);
        }

        public void Transfer(TransferClientData data)
        {
            var receiverAcc = _userManager.FindById(data.Receiver);
            Try.NotNull(receiverAcc, $"Не найден пользователь {data.Receiver}.");

            var senderAcc = _userManager.FindById(data.Sender);
            Try.NotNull(senderAcc, $"Не найден пользователь {data.Sender}.");
            Try.Condition(data.Amount > 0, $"Неверная сумма перевода");

            _rightsManager.CheckForAccessOverSlave(senderAcc, AccountAccessRoles.Withdraw);
            data.Description = data.Description ?? "";

            var trList = new List<Transaction>();
                
            if ((receiverAcc.Role & AccountRole.Person) > 0 && (senderAcc.Role & AccountRole.Person) > 0)
                trList = C2CTransfer(senderAcc, receiverAcc, data);
            else if((receiverAcc.Role & AccountRole.Company) > 0 && (senderAcc.Role & AccountRole.Person) > 0)
                trList = C2BTransfer(senderAcc, receiverAcc, data);
            else if ((receiverAcc.Role & AccountRole.Corp) > 0 && (senderAcc.Role & AccountRole.Person) > 0)
                trList = C2BTransfer(senderAcc, receiverAcc, data);
            else
                trList = B2BTransfer(senderAcc, receiverAcc, data);

            using (var dbTransact = UserContext.Data.Database.BeginTransaction())
            {
                UserContext.Data.BeginFastSave();

                trList.ForEach(x =>
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

        public float GetTax(string sender, string receiver)
        {
            var receiverAcc = _userManager.FindById(receiver);
            Try.NotNull(receiverAcc, $"Не найден пользователь {receiver}.");
            Try.Condition((receiverAcc.Role & AccountRole.Person) > 0, 
                $"Пользователь {receiver} - не персона");

            var senderAcc = _userManager.FindById(sender);
            Try.NotNull(senderAcc, $"Не найден пользователь {sender}.");
            Try.Condition((senderAcc.Role & AccountRole.Person) > 0,
                $"Пользователь {sender} - не персона");
            _rightsManager.CheckForAccessOverSlave(senderAcc, AccountAccessRoles.Read);

            return GetC2CTax(senderAcc, receiverAcc);
        }

        //Транзакции между физлицами
        //Налог платит отправитель в зависимости от разницы страховок
        private List<Transaction> C2CTransfer(Account sender, Account receiver, TransferClientData data)
        {
            Try.Condition(sender.Cash >= data.Amount, $"Недостаточно средств");
            var ret = new List<Transaction>();
            var tax = GetC2CTax(sender, receiver);
            if (tax > 0)
            {
                var master = _userManager.FindById("master");
                var t = new Transaction(sender, master, data.Amount * tax);
                t.Comment = $"Налог на транзакции физлиц в размере {tax * 100}%";
                t.Type = TransactionType.Tax;
                ret.Add(t);
            }
            var transaction = new Transaction(sender, receiver, data.Amount * (1 - tax));
            transaction.Comment = data.Description;
            ret.Add(transaction);
            return ret;
        }

        private float GetC2CTax(Account sender, Account receiver)
        {
            var diff = Math.Max(0, receiver.EffectiveLevel - sender.EffectiveLevel);
            var tax = diff * 0.25f;
            return tax;
        }

        //Транзакции юрлицам, налог платит получатель
        //Работают страховки
        private List<Transaction> C2BTransfer(Account sender, Account receiver, TransferClientData data)
        {
            var ret = new List<Transaction>();
            var level = _insuranceManager.CheckLoyaltyLevel(sender, receiver);
            var discount = _constantManager.GetDiscount(level);

            Try.Condition(sender.Cash >= data.Amount * (1 - discount), $"Недостаточно средств");

            if (discount == 0)
            {
                var master = _userManager.FindById("master"); //UserContext.MasterAcc
                var t = new Transaction(receiver, master, data.Amount * 0.5f);
                t.Comment = $"Налог на доходные предприятия в размере 50%";
                t.Type = TransactionType.Tax;
                ret.Add(t);
            }
            var transaction = new Transaction(sender, receiver, data.Amount * (1 - discount));
            transaction.Comment = data.Description + $" номинал {data.Amount} скидка {discount * 100}%";
            ret.Add(transaction);
            return ret;
        }

        //Транзакции между юолицами и прочие, особых правил нет
        private List<Transaction> B2BTransfer(Account sender, Account receiver, TransferClientData data)
        {
            Try.Condition(sender.Cash >= data.Amount, $"Недостаточно средств");
            var transaction = new Transaction(sender, receiver, data.Amount);
            transaction.Comment = data.Description;
            var ret = new List<Transaction> {transaction};
            return ret;
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