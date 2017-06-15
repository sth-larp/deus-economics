using System;
using System.Collections.Generic;
using System.Linq;
using DeusCloud.Data.Entities.Accounts;
using DeusCloud.Data.Entities.Taxes;
using DeusCloud.Data.Entities.Transactions;
using DeusCloud.Exceptions;
using DeusCloud.Identity;
using DeusCloud.Logic.CommonBase;
using Microsoft.AspNet.Identity;

namespace DeusCloud.Logic.Managers
{
    public class TaxManager : ContextHolder
    {
        private UserManager _userManager;
        private RightsManager _rightsManager;

        public static Dictionary<TaxType, Tax> Taxes { get; protected set; }

        public TaxManager(UserContext context) : base(context)
        {
            _userManager = new UserManager(UserContext);
            _rightsManager = new RightsManager(UserContext);

            if (Taxes == null)
            {
                Taxes = UserContext.Data.Taxes.ToList().ToDictionary(x => x.Type, x => x);
            }
        }

        public List<Transaction> TakeTax(Transaction transaction)
        {
            var ret = new List<Transaction>();

            //Налог на все транзакции
            var government = _userManager.FindById("govt");

            if (transaction.ReceiverAccount.Login != "govt" && government != null
                && Taxes != null && Taxes.ContainsKey(TaxType.Transaction)
                )
            {
                var taxValue = Taxes[TaxType.Transaction].PercentValue;
                var sum = transaction.Amount * taxValue / 100;
                var t = new Transaction(transaction.ReceiverAccount, government, sum);
                t.Type = TransactionType.Tax;
                t.Comment = String.Format("Налог c транзакций в размере {0}%", taxValue);
                ret.Add(t);
            }

            //Налог на прибыльные предприятия
            var master = _userManager.FindById("master");
            if ((transaction.ReceiverAccount.Role |= AccountRole.Tavern) > 0 
                && Taxes != null && Taxes.ContainsKey(TaxType.Tavern)
                && master != null)
            {
                var taxValue = Taxes[TaxType.Tavern].PercentValue;
                var sum = transaction.Amount * taxValue / 100;
                var t = new Transaction(transaction.ReceiverAccount, master, sum);
                t.Type = TransactionType.Tax;
                t.Comment = String.Format("Налог на прибыль в размере {0}%", taxValue);
                ret.Add(t);
            }

            ret.Add(transaction);
            return ret;
        }

        public List<Tax> GetTaxes()
        {
            return Taxes.Values.ToList();
        }

        public Tax NewTax(string text, TaxType type, float value)
        {
            _rightsManager.CheckRole(AccountRole.Admin);
            Try.Condition(!Taxes.ContainsKey(type), $"This tax already exests: {type}.");
            var tax = new Tax
            {
                Description = text,
                PercentValue = value,
                Type = type
            };
            Taxes.Add(type, tax);
            UserContext.Data.Taxes.Add(tax);
            UserContext.Data.SaveChanges();
            return tax;
        }

        public Tax EditTax(string text, TaxType type, float value)
        {
            _rightsManager.CheckRole(AccountRole.Admin);

            Try.Condition(Taxes.ContainsKey(type), $"Cant find tax with type: {type}.");
            var tax = Taxes[type];
            tax.Description = text;
            tax.PercentValue = value;

            var dbtax = UserContext.Data.Taxes.Find(type);
            dbtax.Description = text;
            dbtax.PercentValue = value;
            UserContext.Data.SaveChanges();
            return tax;
        }

        public void DeleteTax(TaxType type)
        {
            _rightsManager.CheckRole(AccountRole.Admin);

            Try.Condition(Taxes.ContainsKey(type), $"Cant find tax with type: {type}.");
            Taxes.Remove(type);

            var tax = UserContext.Data.Taxes.First(c => c.Type == type);
            UserContext.Data.Taxes.Remove(tax);
            UserContext.Data.SaveChanges();
        }
    }
}