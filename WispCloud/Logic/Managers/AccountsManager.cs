using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
    public sealed class AccountsManager : ContextHolder
    {
        UserManager _userManager;
        private RightsManager _rightsManager;
        private ConstantManager _constantManager;

        public AccountsManager(UserContext context)
            : base(context)
        {
            _userManager = new UserManager(UserContext);
            _rightsManager = new RightsManager(UserContext);
            _constantManager = new ConstantManager(UserContext);
        }

        public Account Registration(RegistrationClientData clientData)
        {
            _rightsManager.CheckRole(AccountRole.Admin);

            var role = clientData.Role ?? AccountRole.Person;
            var newUser = new Account(clientData.Login, role);
           
            newUser.Fullname = clientData.Fullname ?? "Нет имени";
            newUser.Email = clientData.Email ?? "";
            newUser.Cash = clientData.Cash ?? 0;
            newUser.Insurance = clientData.Insurance ?? InsuranceType.None;
            newUser.InsuranceLevel = newUser.Insurance.SetLevel(clientData.InsuranceLevel);
            
            var result = _userManager.Create(newUser, clientData.Password);
            if (!result.Succeeded)
                throw new DeusException(result.Errors.First());

            UserContext.AddGameEvent(clientData.Login, GameEventType.None, $"Аккаунт создан");

            if (!String.IsNullOrEmpty(clientData.Workplace) && clientData.SalaryLevel != null)
            {
                var workPlace = _userManager.FindById(clientData.Workplace);
                if (workPlace == null)
                {
                    UserContext.AddGameEvent(clientData.Login, GameEventType.None, 
                        $"Не удалось добавить место работы {clientData.Workplace}");
                }
                else
                {
                    var salary = _constantManager.GetSalary(clientData.SalaryLevel.Value);
                    var payment = new Payment(workPlace, newUser, salary);
                    UserContext.Data.Payments.Add(payment);
                    UserContext.Data.SaveChanges();
                }
            }

            return newUser;
        }

        public void ChangePassword(ChangePasswordClientData clientData)
        {
            var account = _userManager.FindById(clientData.Login);
            Try.NotNull(account, $"Cant find account with login: {clientData.Login}.");

            IdentityResult result;
            if (clientData.Login != UserContext.CurrentUser.Login)
            {
                Try.Condition((UserContext.CurrentUser.Role | AccountRole.Admin) > 0,
                    $"Only Administrators can change user account password: {clientData.Login}.");
                result = _userManager.NewPassword(clientData.Login, clientData.NewPassword);
            }
            else
            {
                result = _userManager.ChangePassword(clientData.Login, clientData.CurrentPassword,
                    clientData.NewPassword);
            }

            if (!result.Succeeded)
                throw new DeusException(result.Errors.First());

            UserContext.AddGameEvent(account.Login, GameEventType.None, $"Изменен пароль");
        }

        public Account GetProfile(string login)
        {
            return _rightsManager.CheckForAccessOverSlave(login, AccountAccessRoles.Read);
        }

        public Account Get(string login)
        {
            return _userManager.FindById(login);
        }

        public Account Get(string login, string password)
        {
            return _userManager.Find(login, password);
        }

        public void Update(Account account)
        {
            var result = _userManager.Update(account);
            if (!result.Succeeded)
                throw new DeusException(result.Errors.First());
        }

        public List<Account> GetAccountList()
        {
            _rightsManager.CheckRole(AccountRole.Admin);
            var res = UserContext.Data.Accounts.ToList();
            return res;
        }

        public FullAccountServerData GetFullProfile(string login)
        {
            var user = _rightsManager.CheckForAccessOverSlave(login, AccountAccessRoles.Read);
            var data = new FullAccountServerData(user);
            data.History = UserContext.Data.GameEvents.Where(x => x.User == user.Login).ToList();
            return data;
        }
    }
}