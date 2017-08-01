using System.Collections.Generic;
using System.Linq;
using DeusCloud.Data.Entities.Accounts;
using DeusCloud.Data.Entities.Constants;
using DeusCloud.Exceptions;
using DeusCloud.Logic.CommonBase;

namespace DeusCloud.Logic.Managers
{
    public class ConstantManager : ContextHolder
    {
        public static Dictionary<string, Constant> Constants { get; protected set; }

        public ConstantManager(UserContext context) : base(context)
        {
            if (Constants == null)
            {
                Constants = UserContext.Data.Constants.ToList().ToDictionary(x => x.Name, x => x);
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

        public float GetSalary(int salaryLevel)
        {
            if (salaryLevel == 1) return 10;
            if (salaryLevel == 2) return 50;
            if (salaryLevel == 3) return 200;
            return 0;
        }

        public float GetInsuranceCost(InsuranceType type, int level)
        {
            if (type == InsuranceType.None) return 0;
            if (type == InsuranceType.SuperVip) return 0;

            var costs = new float[]
            {
                0,
                Constants.ContainsKey("InsCost1") ? Constants["InsCost1"].Value : 1,
                Constants.ContainsKey("InsCost2") ? Constants["InsCost2"].Value : 2,
                Constants.ContainsKey("InsCost3") ? Constants["InsCost3"].Value : 3,
            };

            return costs[level];
        }

        public float GetInsuranceSalary(InsuranceType type, int level)
        {
            if (type == InsuranceType.None) return 0;
            if (type == InsuranceType.SuperVip) return 100;
            if (type == InsuranceType.Govt) return level * 20;
            return level * 20;
        }

        public Constant NewConstant(string text, string name, float value)
        {
            UserContext.Rights.CheckRole(AccountRole.Admin);
            Try.Condition(!Constants.ContainsKey(name), $"Эта константа уже существует: {name}.");
            var c = new Constant
            {
                Description = text,
                Value = value,
                Name = name
            };
            Constants.Add(name, c);
            UserContext.Data.Constants.Add(c);
            UserContext.Data.SaveChanges();
            return c;
        }

        public Constant EditConstant(string text, string name, float value)
        {
            UserContext.Rights.CheckRole(AccountRole.Admin);

            Try.Condition(Constants.ContainsKey(name), $"Не найдена константа: {name}.");
            var c = Constants[name];
            c.Description = text;
            c.Value = value;

            var dbconst = UserContext.Data.Constants.Find(name);
            dbconst.Description = text;
            dbconst.Value = value;
            UserContext.Data.SaveChanges();
            return c;
        }

        public void DeleteConstant(string name)
        {
            UserContext.Rights.CheckRole(AccountRole.Admin);

            Try.Condition(Constants.ContainsKey(name), $"Не найдена константа: {name}.");
            Constants.Remove(name);

            var c = UserContext.Data.Constants.First(x => x.Name == name);
            UserContext.Data.Constants.Remove(c);
            UserContext.Data.SaveChanges();
        }

        
    }
}