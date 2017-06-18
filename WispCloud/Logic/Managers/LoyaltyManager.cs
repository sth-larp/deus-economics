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
    public class LoyaltyManager : ContextHolder
    {
        private UserManager _userManager;
        private RightsManager _rightsManager;

        private static Dictionary<string, InsuranceType> _associations = 
            new Dictionary<string, InsuranceType>
        {
            {"govt", InsuranceType.Govt },
            {"JJ", InsuranceType.JJ},
            {"Serenity", InsuranceType.Serenity},
            {"Corp3", InsuranceType.Corp3},
            {"admin", InsuranceType.SuperVip}
        }; 

        public LoyaltyManager(UserContext context) : base(context)
        {
            _userManager = new UserManager(UserContext);
            _rightsManager = new RightsManager(UserContext);
        }

        public List<Loyalty> GetLoyalties()
        {
            _rightsManager.CheckRole(AccountRole.Admin);

            return UserContext.Data.Loyalties.ToList();
        }

        public void DeleteLoyalty(int id)
        {
            _rightsManager.CheckRole(AccountRole.Admin);

            var loyalty = UserContext.Data.Loyalties.Find(id);
            Try.NotNull(loyalty, $"Can't find insurance loyalty relation with id: {id}");

            UserContext.Data.Loyalties.Remove(loyalty);
            UserContext.Data.SaveChanges();
        }

        public Loyalty NewLoyalty(Loyalty data)
        {
            _rightsManager.CheckRole(AccountRole.Admin);

            var company = _userManager.FindById(data.LoyalName);
            Try.NotNull(company, $"Не найден логин: {company}");
            Try.Condition((company.Role & AccountRole.Tavern) > 0, 
                $"{company} не является организацией типа {AccountRole.Tavern}");

            var check = UserContext.Data.Loyalties.Where(x =>
                x.LoyalName == company.Login && x.Insurance == data.Insurance);
            Try.Condition(!check.Any(), $"Компания уже обслуживает данную страховку");
            Try.Condition(data.MinLevel < 4 && data.MinLevel > 0, $"Значения MinLevel разрешены в диапазоне [1:3]");

            var loyalty = new Loyalty(company, data.Insurance);
            loyalty.MinLevel = data.MinLevel;

            UserContext.Data.Loyalties.Add(loyalty);
            UserContext.Data.SaveChanges();

            return loyalty;
        }

        public int CheckLoyaltyLevel(Account person, Account service)
        {
            var check = UserContext.Data.Loyalties.Where(x =>
                x.LoyalName == service.Login && x.Insurance == person.Insurance);
            if (check.Any())
                return person.InsuranceLevel;
            return 0;
        }

        public List<Loyalty> GetCompanyLoyalties(string login)
        {
            _rightsManager.CheckForAccessOverSlave(login, AccountAccessRoles.Read);

            return UserContext.Data.Loyalties.Where(x => x.LoyalService.Login == login).ToList();
        }

        public List<InsuranceHolderServerData> GetLoyaltyHolders(string login)
        {
            var ret = new List<InsuranceHolderServerData>();
            _rightsManager.CheckForAccessOverSlave(login, AccountAccessRoles.Read);
            Try.Condition(_associations.ContainsKey(login), $"{login} не выпускает страховки");
            var t = _associations[login];
            var list = UserContext.Data.Accounts.Where(x => x.Insurance == t);

            foreach (var user in list)
            {
                var h = new InsuranceHolderServerData(user.Login, login)
                {
                    InsuranceLevel = user.InsuranceLevel,
                    Insurance = t,
                    UserFullname = user.Fullname??""
                };
                ret.Add(h);
            }
            return ret;
        }

        public void RemoveLoyaltyHolder(string company, string user)
        {
            _rightsManager.CheckForAccessOverSlave(company, AccountAccessRoles.Withdraw);
            Try.Condition(_associations.ContainsKey(company), $"{company} не выпускает страховки");
            var t = _associations[company];
            var userAccount = _userManager.FindById(user);

            Try.Condition(userAccount.Insurance == t, $"{user} не имеет нужной страховки");
            userAccount.Insurance = InsuranceType.None;
            UserContext.Accounts.Update(userAccount);
        }
    }
}