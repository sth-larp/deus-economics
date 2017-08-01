using System;
using System.Collections.Generic;
using System.Linq;
using DeusCloud.Data.Entities.Access;
using DeusCloud.Data.Entities.Accounts;
using DeusCloud.Data.Entities.GameEvents;
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
        private RightsManager _rightsManager;
        private ConstantManager _constantManager;
        private InsuranceManager _insuranceManager;

        public TransactionsManager(UserContext context) : base(context)
        {
            _rightsManager = new RightsManager(UserContext);
            _constantManager = new ConstantManager(UserContext);
            _insuranceManager = new InsuranceManager(UserContext);
        }

        public float Transfer(TransferClientData data)
        {
            var receiverAcc = UserContext.Accounts.GetOrFail(data.Receiver, true); //Разрешен Alias
            var senderAcc = UserContext.Accounts.GetOrFail(data.Sender); 

            Try.Condition(receiverAcc.Login != senderAcc.Login, "Нельзя переводить самому себе");

            _rightsManager.CheckForAccessOverSlave(senderAcc, AccountAccessRoles.Withdraw);
            data.Description = data.Description ?? "";

            var trList = new List<Transaction>();
            
            //Разные виды налогообложения    
            if (receiverAcc.Role == AccountRole.Person && senderAcc.Role == AccountRole.Person)
                trList = P2PTransfer(senderAcc, receiverAcc, data);
            else if(receiverAcc.Role.IsCompany() && senderAcc.Role == AccountRole.Person)
                trList = P2BTransfer(senderAcc, receiverAcc, data);
            else
                trList = B2BTransfer(senderAcc, receiverAcc, data);

            //Анонимные транзакции
            if(data.Receiver.ToLower() == receiverAcc.Alias.ToLower() && receiverAcc.Role == AccountRole.Person)
                trList.ForEach(x => {
                    if(x.ReceiverAccount.Login == receiverAcc.Login)
                        x.Type |= TransactionType.Anonymous;
                });

            using (var dbTransact = UserContext.Data.Database.BeginTransaction())
            {
                UserContext.Data.BeginFastSave();
                trList.ForEach(TransactiontoDb);
                dbTransact.Commit();
            }
            return senderAcc.Cash;
        }

        private void TransactiontoDb(Transaction t)
        {
            t.SenderAccount.Cash -= t.Amount;
            t.ReceiverAccount.Cash += t.Amount;

            UserContext.Accounts.Update(t.SenderAccount);
            UserContext.Accounts.Update(t.ReceiverAccount);

            UserContext.Data.Transactions.Add(t);
            UserContext.Data.SaveChanges();
        }

        public void Implant(ImplantClientData data)
        {
            var receiverAcc = UserContext.Accounts.GetOrFail(data.Receiver, data.ReceiverPass);
            var sellerAcc = UserContext.Accounts.GetOrFail(data.Seller);
            var parentAcc = UserContext.Accounts.Get(sellerAcc.ParentID) ?? sellerAcc;

            _rightsManager.CheckForAccessOverSlave(sellerAcc, AccountAccessRoles.Withdraw);
            Try.Condition(parentAcc.Role.IsCompany(), $"Продавать импланты может только компания");
            Try.Condition(receiverAcc.Role == AccountRole.Person, $"Получать импланты может только персона");

            int indexCost = data.Index;
            if (receiverAcc.Insurance == InsuranceType.SuperVip
                || parentAcc == UserContext.Insurances.GetIssuerFromType(receiverAcc.Insurance))
            {
                var discount = UserContext.Constants.GetDiscount(receiverAcc.EffectiveLevel);
                indexCost = (int) Math.Ceiling(indexCost*(1 - discount));
            }

            Try.Condition(parentAcc.Index >= indexCost, $"Недостаточно индекса");

            var t = new Transaction(receiverAcc, sellerAcc, data.Price);
            t.Comment = data.Description ?? "";
            t.Type |= TransactionType.Implant;

            using (var dbTransact = UserContext.Data.Database.BeginTransaction())
            {
                UserContext.Data.BeginFastSave();
                TransactiontoDb(t);
                parentAcc.Index -= indexCost;
                dbTransact.Commit();
            }

            if(parentAcc != sellerAcc)
                UserContext.AddGameEvent(parentAcc.Login, GameEventType.Index, 
                    $"Имплант за {indexCost} индекса установлен магазином {sellerAcc.Fullname}", true);

            UserContext.AddGameEvent(sellerAcc.Login, GameEventType.Index,
                $"Имплант за {indexCost} индекса установлен {receiverAcc.Fullname}", true);

            UserContext.AddGameEvent(receiverAcc.Login, GameEventType.Index,
                $"Получен имплант от {sellerAcc.Fullname}", true);
        }

        
        //Транзакции между физлицами
        //Налог платит отправитель в зависимости от разницы страховок
        private List<Transaction> P2PTransfer(Account sender, Account receiver, TransferClientData data)
        {
            Try.Condition(sender.Cash >= data.Amount, $"Недостаточно средств");
            var ret = new List<Transaction>();
            var tax = GetP2pTax(sender, receiver);
            if (tax > 0)
            {
                var master = UserContext.Accounts.GetOrFail("admin");
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

        private float GetP2pTax(Account sender, Account receiver)
        {
            var diff = Math.Max(0, receiver.EffectiveLevel - sender.EffectiveLevel);
            var tax = UserContext.Constants.GetDiscount(diff);
            return tax;
        }

        //Транзакции юрлицам, налог платит получатель
        //Работают страховки
        private List<Transaction> P2BTransfer(Account sender, Account receiver, TransferClientData data)
        {
            var ret = new List<Transaction>();
            var level = _insuranceManager.CheckInsuranceGrade(sender, receiver);
            var discount = _constantManager.GetDiscount(level);

            Try.Condition(sender.Cash >= data.Amount * (1 - discount), $"Недостаточно средств");

            if (discount == 0)
            {
                var master = UserContext.Accounts.GetOrFail("admin");
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
            var user = UserContext.Accounts.GetOrFail(login);

            Try.Condition(skip >= 0, $"Поле {nameof(skip)} не должно быть отрицательным");
            Try.Condition(take >= 0, $"Поле {nameof(take)} не должно быть отрицательным");

            _rightsManager.CheckForAccessOverSlave(user, AccountAccessRoles.Read);

            var ret = UserContext.Data.Transactions
                .Where(x => x.Receiver == user.Login || x.Sender == user.Login)
                .OrderByDescending(x => x.Time).Skip(skip).Take(take).ToList();

            ret.ForEach(x =>{ x.HideIfRequired(); });
            return ret.ToList();
        }
    }
}