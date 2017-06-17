using System.Collections.Generic;
using System.Linq;
using DeusCloud.Data.Entities.Accounts;
using DeusCloud.Data.Entities.Transactions;
using DeusCloud.Exceptions;
using DeusCloud.Identity;
using DeusCloud.Logic.CommonBase;
using Microsoft.AspNet.Identity;

namespace DeusCloud.Logic.Managers
{
    public class LoyaltyManager : ContextHolder
    {
        private UserManager _userManager;
        private RightsManager _rightsManager;

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
            Try.NotNull(company, $"Can't find account with id: {company}");
            Try.Condition((company.Role & AccountRole.Tavern) > 0, 
                $"Service account is not of required type: {company.Role}");

            var check = UserContext.Data.Loyalties.Where(x =>
                x.LoyalName == company.Login && x.Insurance == data.Insurance);
            Try.Condition(!check.Any(), $"This relation already exists");

            var loyalty = new Loyalty(company, data.Insurance);

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
    }
}