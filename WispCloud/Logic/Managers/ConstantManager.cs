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
        private RightsManager _rightsManager;
        private InsuranceManager _insuranceManager;

        public static Dictionary<ConstantType, Constant> Constants { get; protected set; }

        public ConstantManager(UserContext context) : base(context)
        {
            _rightsManager = new RightsManager(UserContext);
            _insuranceManager = new InsuranceManager(UserContext);

            if (Constants == null)
            {
                Constants = UserContext.Data.Constants.ToList().ToDictionary(x => x.Type, x => x);
            }
        }

        public List<Constant> GetConstants()
        {
            return Constants.Values.ToList();
        }

        public float GetDiscount(int level)
        {
            if (level == 1) return 0.25f;
            if (level == 2) return 0.5f;
            if (level == 3) return 0.75f;
            return 0;
        }

        public float GetSalary(int level)
        {
            if (level == 1) return 10;
            if (level == 2) return 50;
            if (level == 3) return 200;
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