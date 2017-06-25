using System;
using System.Collections.Generic;
using System.Linq;
using DeusCloud.Data.Entities.Accounts;
using DeusCloud.Data.Entities.Constants;
using DeusCloud.Data.Entities.Transactions;
using DeusCloud.Exceptions;
using DeusCloud.Identity;
using DeusCloud.Logic.CommonBase;
using Microsoft.AspNet.Identity;

namespace DeusCloud.Logic.Managers
{
    public class ConstantManager : ContextHolder
    {
        private UserManager _userManager;
        private RightsManager _rightsManager;
        private InsuranceManager _insuranceManager;

        public static Dictionary<ConstantType, Constant> Constants { get; protected set; }

        public ConstantManager(UserContext context) : base(context)
        {
            _userManager = new UserManager(UserContext);
            _rightsManager = new RightsManager(UserContext);
            _insuranceManager = new InsuranceManager(UserContext);

            if (Constants == null)
            {
                Constants = UserContext.Data.Constants.ToList().ToDictionary(x => x.Type, x => x);
            }
        }

        public List<Transaction> TakeTax(Transaction transaction)
        {
            var ret = new List<Transaction>();

            //Налог на все транзакции
            /*var government = _userManager.FindById("govt");

            if (transaction.ReceiverAccount.Login != "govt" && government != null
                && Constants != null && Constants.ContainsKey(ConstantType.Transaction))
            {
                var taxValue = Constants[ConstantType.Transaction].PercentValue;
                var sum = transaction.Amount * taxValue / 100;
                var t = new Transaction(transaction.ReceiverAccount, government, sum);
                t.Type = TransactionType.Tax;
                t.Comment = String.Format("Налог c транзакций в размере {0}%", taxValue);
                ret.Add(t);
            }*/

            //Налог на прибыльные предприятия
            var master = _userManager.FindById("master");
            if ((transaction.ReceiverAccount.Role & AccountRole.Tavern) > 0 
                && Constants != null && Constants.ContainsKey(ConstantType.TavernTax)
                && master != null)
            {
                var level = _insuranceManager.CheckLoyaltyLevel(transaction.SenderAccount,
                    transaction.ReceiverAccount);
                var discount = GetDiscount(level);

                var constValue = Constants[ConstantType.TavernTax].PercentValue / 100 - (float)discount;
                if (constValue > 0)
                {
                    var sum = transaction.Amount * constValue;
                    var t = new Transaction(transaction.ReceiverAccount, master, sum);
                    t.Type = TransactionType.Tax;
                    t.Comment = "Налог на прибыльные заведения";
                    ret.Add(t);
                }

                if (level > 0)
                {
                    transaction.Comment += $" номинал {transaction.Amount} со скидкой {discount*100}% за счет страховки";
                    transaction.Type |= TransactionType.Insurance;
                }

                transaction.Amount *= (1f - (float)discount);
            }

            ret.Add(transaction);
            return ret;
        }

        public List<Constant> GetConstants()
        {
            return Constants.Values.ToList();
        }

        private decimal GetDiscount(int level)
        {
            if (level == 1) return (decimal) 0.25;
            if (level == 2) return (decimal) 0.5;
            if (level == 3) return (decimal) 0.75;
            return 0;
        }

        public Constant NewConstant(string text, ConstantType type, float value)
        {
            _rightsManager.CheckRole(AccountRole.Admin);
            Try.Condition(!Constants.ContainsKey(type), $"This constant already exists: {type}.");
            var c = new Constant
            {
                Description = text,
                PercentValue = value,
                Type = type
            };
            Constants.Add(type, c);
            UserContext.Data.Constants.Add(c);
            UserContext.Data.SaveChanges();
            return c;
        }

        public Constant EditConstant(string text, ConstantType type, float value)
        {
            _rightsManager.CheckRole(AccountRole.Admin);

            Try.Condition(Constants.ContainsKey(type), $"Cant find constant with type: {type}.");
            var c = Constants[type];
            c.Description = text;
            c.PercentValue = value;

            var dbconst = UserContext.Data.Constants.Find(type);
            dbconst.Description = text;
            dbconst.PercentValue = value;
            UserContext.Data.SaveChanges();
            return c;
        }

        public void DeleteConstant(ConstantType type)
        {
            _rightsManager.CheckRole(AccountRole.Admin);

            Try.Condition(Constants.ContainsKey(type), $"Cant find constant with type: {type}.");
            Constants.Remove(type);

            var c = UserContext.Data.Constants.First(x => x.Type == type);
            UserContext.Data.Constants.Remove(c);
            UserContext.Data.SaveChanges();
        }
    }
}