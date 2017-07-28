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

        private Dictionary<string, string> _regAlias = new Dictionary<string, string>
        {
            {"johnsoncorp","JJ"},
            {"kintsugicorp","Serenity"},
            {"panamcorp","Panam"},
            {"sungov","Govt"},
        };

        public Account Registration(RegistrationClientData clientData)
        {
            _rightsManager.CheckRole(AccountRole.Admin);

            var existing = Get(clientData.Login);
            if (existing != null)
                return UpdateExisting(existing, clientData);

            var role = clientData.Role ?? AccountRole.Person;
            var newUser = new Account(clientData.Login, role);
           
            newUser.Fullname = clientData.Fullname ?? "Нет имени";

            if(!String.IsNullOrEmpty(clientData.Email))
                newUser.Email = clientData.Email;

            newUser.Cash = clientData.Cash ?? 0;
            newUser.Insurance = clientData.Insurance ?? InsuranceType.None;
            newUser.InsuranceLevel = newUser.Insurance.SetLevel(clientData.InsuranceLevel);
            
            var result = _userManager.Create(newUser, clientData.Password);
            if (!result.Succeeded)
                throw new DeusException(result.Errors.First());

            UserContext.AddGameEvent(clientData.Login, GameEventType.None, $"Аккаунт создан");

            if (!String.IsNullOrEmpty(clientData.Workplace) && clientData.SalaryLevel != null)
            {
                if (_regAlias.ContainsKey(clientData.Workplace))
                    clientData.Workplace = _regAlias[clientData.Workplace];

                var workPlace = Get(clientData.Workplace);
                Try.NotNull(workPlace, $"Не удалось добавить место работы {clientData.Workplace}, счет {clientData.Login}");

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

        public Account UpdateExisting(Account user, RegistrationClientData clientData)
        {
            user.Fullname = clientData.Fullname ?? user.Fullname;

            if (!String.IsNullOrEmpty(clientData.Email))
                user.Email = clientData.Email;

            user.Cash = clientData.Cash ?? user.Cash;
            user.Insurance = clientData.Insurance ?? user.Insurance;

            if(clientData.InsuranceLevel != null)
                user.InsuranceLevel = user.Insurance.SetLevel(clientData.InsuranceLevel);

            if (clientData.Password != null)
                _userManager.NewPassword(user.Login, clientData.Password);

            UserContext.AddGameEvent(clientData.Login, GameEventType.None, $"Аккаунт изменен при импорте Join");

            if (!String.IsNullOrEmpty(clientData.Workplace) && clientData.SalaryLevel != null)
            {
                if (_regAlias.ContainsKey(clientData.Workplace))
                    clientData.Workplace = _regAlias[clientData.Workplace];

                var workPlace = Get(clientData.Workplace);
                Try.NotNull(workPlace, $"Не удалось добавить место работы {clientData.Workplace}, счет {clientData.Login}");
                if (workPlace == null)
                {
                    UserContext.AddGameEvent(clientData.Login, GameEventType.None,
                        $"Не удалось добавить место работы {clientData.Workplace}");
                }
                else
                {
                    var oldPayments = UserContext.Data.Payments.Where(x => x.Receiver == user.Login).ToList();
                    oldPayments.ForEach(x => UserContext.Data.Payments.Remove(x));

                    var salary = _constantManager.GetSalary(clientData.SalaryLevel.Value);
                    var payment = new Payment(workPlace, user, salary);
                    UserContext.Data.Payments.Add(payment);
                    UserContext.Data.SaveChanges();
                }
            }

            return user;
        }


        public void ChangePassword(ChangePasswordClientData clientData)
        {
            var account = GetOrFail(clientData.Login);

            IdentityResult result;
            if (account.Login != UserContext.CurrentUser.Login)
            {
                //Only Administrators can change user account password
                _rightsManager.CheckRole(AccountRole.Admin);

                result = _userManager.NewPassword(account.Login, clientData.NewPassword);
            }
            else
            {
                result = _userManager.ChangePassword(account.Login, clientData.CurrentPassword,
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

        public Account Get(string login, bool allowAlias = false)
        {
            if (String.IsNullOrEmpty(login)) return null;

            var account = _userManager.FindById(login);
            if (account == null)
                account = UserContext.Data.Accounts.FirstOrDefault(x => x.Email == login);
            if (allowAlias && account == null)
                account = UserContext.Data.Accounts.FirstOrDefault(x => x.Alias == login);
            return account;
            //return _userManager.FindById(login);
        }

        public Account GetOrFail(string login, bool allowAlias = false)
        {
            var account = Get(login, allowAlias);
            Try.NotNull(account, $"Не найден счет {login}");
            return account;
        }

        public Account GetOrFail(string login, string password)
        {
            var account = Get(login);
            if (account != null) 
                account = _userManager.Find(account.Login, password);
            Try.NotNull(account, $"Неверное имя или пароль {login}");
            return account;
        }

        public void Update(Account account)
        {
            var result = _userManager.Update(account);
            if (!result.Succeeded)
                throw new DeusException(result.Errors.First());
        }

        public List<Account> GetAccountList()
        {
            _rightsManager.CheckRole(AccountRole.Admin | AccountRole.Master);
            var res = UserContext.Data.Accounts.ToList();
            return res;
        }

        public FullAccountServerData GetFullProfile(string login)
        {
            var user = _rightsManager.CheckForAccessOverSlave(login, AccountAccessRoles.Read);
            var data = new FullAccountServerData(user);
            data.History = UserContext.Data.GameEvents
                .Where(x => x.User == user.Login)
                .OrderByDescending(x => x.Time).ToList();
            return data;
        }
    }
}