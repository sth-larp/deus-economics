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
using Microsoft.AspNet.Identity;

namespace DeusCloud.Logic.Managers
{
    public class PaymentsManager : ContextHolder
    {
        private UserManager _userManager;
        private RightsManager _rightsManager;

        public PaymentsManager(UserContext context) : base(context)
        {
            _userManager = new UserManager(UserContext);
            _rightsManager = new RightsManager(UserContext);
        }

        public List<Payment> GetPayments(string login)
        {
            _rightsManager.CheckForAccessOverSlave(login, AccountAccessRoles.Read);
            var ret = UserContext.Data.Payments.Where(
                x => x.Receiver == login || x.Sender == login);
            return ret.ToList();
        }

        public Payment NewPayment(PaymentClientData data)
        {
            _rightsManager.CheckForAccessOverSlave(data.Sender, AccountAccessRoles.Withdraw);
            var senderAcc = _userManager.FindById(data.Sender);
            var receiverAcc = _userManager.FindById(data.Receiver);

            var payment = new Payment(senderAcc, receiverAcc, data.Amount);
            UserContext.Data.Payments.Add(payment);
            UserContext.Data.SaveChanges();

                return payment;
            }

        public Payment EditPayment(int id, PaymentClientData data)
        {
            _rightsManager.CheckForAccessOverSlave(data.Sender, AccountAccessRoles.Withdraw);
            Payment ret = null;
            using (var dbTransact = UserContext.Data.Database.BeginTransaction())
            {
                UserContext.Data.BeginFastSave();

                ret = UserContext.Data.Payments.Find(id);
                Try.NotNull(ret, $"Can't find payment with id: {id}");

                ret.Sender = data.Sender;
                ret.Receiver = data.Receiver;
                ret.Amount = data.Amount;


                UserContext.Data.SaveChanges();
                dbTransact.Commit();
            }
            return ret;
        }

        public void DeletePayment(int id)
        {
            var payment = UserContext.Data.Payments.Find(id);
            Try.NotNull(payment, $"Can't find payment with id: {id}");
            _rightsManager.CheckForAccessOverSlave(payment.Sender, AccountAccessRoles.Withdraw);

            UserContext.Data.Payments.Remove(payment);
            UserContext.Data.SaveChanges();
        }

        public void Run()
        {
            _rightsManager.CheckRole(AccountRole.Admin);

            using (var dbTransact = UserContext.Data.Database.BeginTransaction())
            {
                UserContext.Data.BeginFastSave();

                var list = UserContext.Data.Payments.ToList();
                list.ForEach(PerformPayment);
                
                UserContext.Data.SaveChanges();
                dbTransact.Commit();
            }
        }

        private void PerformPayment(Payment pay)
        {
            if (pay.SenderAccount.Cash < pay.Amount)
            {
                pay.Debt += pay.Amount;
                return;
            }

            pay.SenderAccount.Cash -= pay.Amount;
            pay.ReceiverAccount.Cash += pay.Amount;
            pay.LastPaid = DateTime.Now;

            UserContext.Accounts.Update(pay.SenderAccount);
            UserContext.Accounts.Update(pay.ReceiverAccount);

            var transaction = new Transaction(pay.SenderAccount, pay.ReceiverAccount, pay.Amount);
            transaction.Type |= TransactionType.Payment;

            var taxedTransactions = TaxManager.TakeTax(transaction);
            taxedTransactions.ForEach(x => UserContext.Data.Transactions.Add(x));

            //UserContext.Data.Transactions.Add(transaction);
        }
    }
}