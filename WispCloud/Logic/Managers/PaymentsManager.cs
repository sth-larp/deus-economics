﻿using System;
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

namespace DeusCloud.Logic.Managers
{
    public class PaymentsManager : ContextHolder
    {
        private RightsManager _rightsManager;

        public PaymentsManager(UserContext context) : base(context)
        {
            _rightsManager = new RightsManager(UserContext);
        }

        public List<Payment> GetSalaries(string login)
        {
            var acc = _rightsManager.CheckForAccessOverSlave(login, AccountAccessRoles.Read);
            var ret = UserContext.Data.Payments.Where(x => x.Receiver == acc.Login);
            return ret.ToList();
        }

        public List<Payment> GetPayments(string login)
        {
            var acc = _rightsManager.CheckForAccessOverSlave(login, AccountAccessRoles.Read);
            var ret = UserContext.Data.Payments.Where(x => x.Employer == acc.Login);
            return ret.ToList();
        }

        public List<Payment> GetAllPayments()
        {
            _rightsManager.CheckRole(AccountRole.Admin | AccountRole.Master);
            return UserContext.Data.Payments.ToList();
        }

        public Payment NewPayment(PaymentClientData data)
        {
            _rightsManager.CheckRole(AccountRole.Admin);

            var senderAcc = UserContext.Accounts.GetOrFail(data.Sender);
            var receiverAcc = UserContext.Accounts.GetOrFail(data.Receiver);

            var payment = new Payment(senderAcc, receiverAcc, data.SalaryLevel);
            UserContext.Data.Payments.Add(payment);
            UserContext.Data.SaveChanges();

            LogPaymentEvent(payment);

            return payment;
        }

        public Payment EditPayment(int id, PaymentClientData data)
        {
            _rightsManager.CheckRole(AccountRole.Admin);

            var senderAcc = UserContext.Accounts.GetOrFail(data.Sender);
            var receiverAcc = UserContext.Accounts.GetOrFail(data.Receiver);

            Payment ret = null;
            using (var dbTransact = UserContext.Data.Database.BeginTransaction())
            {
                UserContext.Data.BeginFastSave();

                ret = UserContext.Data.Payments.Find(id);
                Try.NotNull(ret, $"Не удается найти зарплату с Id: {id}");

                ret.Employer = senderAcc.Login;
                ret.Receiver = receiverAcc.Login;
                ret.SalaryLevel = data.SalaryLevel;

                UserContext.Data.SaveChanges();
                dbTransact.Commit();

                LogPaymentEvent(ret);
            }
            return ret;
        }

        public void LogPaymentEvent(Payment payment, bool isDeleted = false)
        {
            if (isDeleted)
            {
                UserContext.AddGameEvent(payment.Receiver, GameEventType.None,
                    $"Отменена зарплата от {payment.EmployerName}");

                UserContext.AddGameEvent(payment.Employer, GameEventType.None,
                    $"Отменена зарплата для {payment.ReceiverName}");

                return;
            }
            UserContext.AddGameEvent(payment.Receiver, GameEventType.None, 
                $"Назначена зарплата от {payment.EmployerName} уровня {payment.SalaryLevel}", true);

            UserContext.AddGameEvent(payment.Employer, GameEventType.None,
                $"Назначена зарплата для {payment.ReceiverName} уровня {payment.SalaryLevel}");
        }

        public void DeletePayment(int id)
        {
            _rightsManager.CheckRole(AccountRole.Admin);

            var payment = UserContext.Data.Payments.Find(id);
            Try.NotNull(payment, $"Не удается найти зарплату с Id: {id}");

            LogPaymentEvent(payment, true);

            UserContext.Data.Payments.Remove(payment);
            UserContext.Data.SaveChanges();
        }

        public void SwitchCycle()
        {
            using (var dbTransact = UserContext.Data.Database.BeginTransaction())
            {
                UserContext.Data.BeginFastSave();

                UserContext.Data.Accounts.ToList().ForEach(x => x.PaidSalary = 0);

                var list = UserContext.Data.Payments.ToList();
                list.ForEach(x =>
                {
                    PerformPayment(x);
                    UserContext.Data.Entry(x).Property(e => e.LastPaid).IsModified = true; //Entity does not track this change
                });
                
                UserContext.Data.SaveChanges();
                dbTransact.Commit();
            }
        }

        public void PerformPayment(Payment pay, bool isInsurance = false, InsuranceType t = InsuranceType.None)
        {
            //pay.EmployerAccount.Cash -= pay.SalaryLevel; //Payment from nowhere
            var value = isInsurance
                ? UserContext.Constants.GetInsuranceSalary(t, pay.SalaryLevel)
                : UserContext.Constants.GetSalary(pay.SalaryLevel);

            if (isInsurance)
            {
                value = Math.Max(0, value - pay.ReceiverAccount.PaidSalary);
            }
            pay.ReceiverAccount.Cash += value;
            pay.ReceiverAccount.PaidSalary += value;
            pay.LastPaid = DateTime.Now;

            var transaction = new Transaction(pay.EmployerAccount, pay.ReceiverAccount, value);
            transaction.Type |= TransactionType.Payment;
            transaction.Comment = $"Выплаты от {pay.EmployerName}" + (isInsurance ? " по страховке. Выплачен max(страховка, зп)" : "");
            UserContext.Data.Transactions.Add(transaction);
        }
    }
}