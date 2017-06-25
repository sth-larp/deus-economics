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
        private ConstantManager _constantManager;

        public PaymentsManager(UserContext context) : base(context)
        {
            _userManager = new UserManager(UserContext);
            _rightsManager = new RightsManager(UserContext);
            _constantManager = new ConstantManager(UserContext);
        }

        public List<Payment> GetPayments(string login)
        {
            _rightsManager.CheckForAccessOverSlave(login, AccountAccessRoles.Read);
            var ret = UserContext.Data.Payments.Where(
                x => x.Receiver == login || x.Employer == login);
            return ret.ToList();
        }

        public List<Payment> GetAllPayments()
        {
            _rightsManager.CheckRole(AccountRole.Admin);
            return UserContext.Data.Payments.ToList();
        }

        public Payment NewPayment(PaymentClientData data)
        {
            _rightsManager.CheckRole(AccountRole.Admin);
            //_rightsManager.CheckForAccessOverSlave(data.Sender, AccountAccessRoles.Withdraw);

            var senderAcc = _userManager.FindById(data.Sender);
            var receiverAcc = _userManager.FindById(data.Receiver);

            var payment = new Payment(senderAcc, receiverAcc, data.Amount);
            UserContext.Data.Payments.Add(payment);
            UserContext.Data.SaveChanges();

            return payment;
        }

        public Payment EditPayment(int id, PaymentClientData data)
        {
            _rightsManager.CheckRole(AccountRole.Admin);
            //_rightsManager.CheckForAccessOverSlave(data.Sender, AccountAccessRoles.Withdraw);

            Payment ret = null;
            using (var dbTransact = UserContext.Data.Database.BeginTransaction())
            {
                UserContext.Data.BeginFastSave();

                ret = UserContext.Data.Payments.Find(id);
                Try.NotNull(ret, $"Can't find payment with id: {id}");

                ret.Employer = data.Sender;
                ret.Receiver = data.Receiver;
                ret.Amount = data.Amount;


                UserContext.Data.SaveChanges();
                dbTransact.Commit();
            }
            return ret;
        }

        public void DeletePayment(int id)
        {
            _rightsManager.CheckRole(AccountRole.Admin);

            var payment = UserContext.Data.Payments.Find(id);
            Try.NotNull(payment, $"Can't find payment with id: {id}");
            //_rightsManager.CheckForAccessOverSlave(payment.Employer, AccountAccessRoles.Withdraw);

            UserContext.Data.Payments.Remove(payment);
            UserContext.Data.SaveChanges();
        }

        public void SwitchCycle()
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
            if (pay.EmployerAccount.Cash < pay.Amount)
            {
                pay.Debt += pay.Amount;
                return;
            }

            //pay.EmployerAccount.Cash -= pay.Amount; //Payment from nowhere
            pay.ReceiverAccount.Cash += pay.Amount;
            pay.LastPaid = DateTime.Now;

            //UserContext.Accounts.Update(pay.EmployerAccount);
            UserContext.Accounts.Update(pay.ReceiverAccount);

            var transaction = new Transaction(pay.EmployerAccount, pay.ReceiverAccount, pay.Amount);
            transaction.Type |= TransactionType.Payment;
            transaction.Comment = $"регулярные выплаты от {pay.EmployerName}";
            UserContext.Data.Transactions.Add(transaction);
        }
    }
}