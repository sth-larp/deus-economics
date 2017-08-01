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
                return SetAccountProperties(clientData);

            var newUser = new Account(clientData.Login, AccountRole.Person);
                  
            var result = _userManager.Create(newUser, clientData.Password);
            Try.Condition(result.Succeeded, $"Не удалось создать счет {clientData.Login}, {result.Errors.FirstOrDefault()}");
            UserContext.Data.SaveChanges();

            UserContext.AddGameEvent(clientData.Login, GameEventType.None, $"Аккаунт создан");
            return SetAccountProperties(clientData);
        }

        public Account SetAccountProperties(RegistrationClientData data)
        {
            UserContext.Rights.CheckRole(AccountRole.Admin);

            var editAccount = UserContext.Accounts.GetOrFail(data.Login);

            if (data.Role != null && data.Role != editAccount.Role)
            {
                editAccount.Role = data.Role.Value;
                UserContext.AddGameEvent(editAccount.Login, GameEventType.Rights, $"Изменен тип счета на {editAccount.Role}");
            }

            if (data.Status != null && data.Status != editAccount.Status)
            {
                editAccount.Status = data.Status.Value;
                UserContext.AddGameEvent(editAccount.Login, GameEventType.Rights, $"Изменен статус счета на {editAccount.Status}");
            }

            if (!String.IsNullOrEmpty(data.Fullname))
                editAccount.Fullname = data.Fullname;

            if (!String.IsNullOrEmpty(data.Email))
                editAccount.Email = data.Email;

            if (!String.IsNullOrEmpty(data.ParentID))
                editAccount.ParentID = data.ParentID;

            if (!String.IsNullOrEmpty(data.Alias))
                editAccount.Alias = data.Alias;

            editAccount.Cash = data.Cash ?? editAccount.Cash;

            if (data.Insurance != null && data.InsuranceLevel != null)
            {
                editAccount.Insurance = data.Insurance.Value;
                editAccount.InsuranceLevel = editAccount.Insurance.SetLevel(data.InsuranceLevel);

            }

            if (!String.IsNullOrEmpty(data.Password))
                _userManager.NewPassword(editAccount.Login, data.Password);

            AddWorkPlace(editAccount, data);

            UserContext.Data.SaveChanges();
            return editAccount;
        }

        private void AddWorkPlace(Account acc, RegistrationClientData data)
        {
            if (String.IsNullOrEmpty(data.Workplace) || data.SalaryLevel == null)
                return;

            if (_regAlias.ContainsKey(data.Workplace))
                data.Workplace = _regAlias[data.Workplace];

            var workPlace = Get(data.Workplace);
            Try.NotNull(workPlace, $"Не удалось добавить место работы {data.Workplace}, счет {acc.Login}");
            if (workPlace == null)
            {
                UserContext.AddGameEvent(acc.Login, GameEventType.None,
                    $"Не удалось добавить место работы {data.Workplace}");
                return;
            }

            var oldPayments = UserContext.Data.Payments.Where(x => x.Receiver == acc.Login).ToList();
            oldPayments.ForEach(x =>
            {
                UserContext.Payments.LogPaymentEvent(x, true);
                UserContext.Data.Payments.Remove(x);
            });

            var payment = new Payment(workPlace, acc, data.SalaryLevel.Value);
            UserContext.Data.Payments.Add(payment);
            UserContext.Payments.LogPaymentEvent(payment);

            if (data.IsAdmin == true)
            {
                UserContext.Rights.SetAccountAccess_Checked(workPlace, acc, AccountAccessRoles.Admin);
            }
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

            Try.Condition(result.Succeeded, $"Ошибка обновления персонажа: {result.Errors.FirstOrDefault()}");

            UserContext.AddGameEvent(account.Login, GameEventType.None, $"Изменен пароль");
        }

        public Account GetProfile(string login)
        {
            var acc = _rightsManager.CheckForAccessOverSlave(login, AccountAccessRoles.Read);
            if (acc.Role.IsCompany())
            {
                var parent = Get(acc.ParentID);
                if (parent != null)
                    acc.Index = parent.Index;
            }
            return acc;
            //DON'T SAVE DATA!!!
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
            Try.Condition(result.Succeeded, $"Ошибка обновления персонажа: {result.Errors.FirstOrDefault()}");
        }

        public List<Account> GetAccountList()
        {
            _rightsManager.CheckRole(AccountRole.Admin | AccountRole.Master);
            var res = UserContext.Data.Accounts.ToList();
            return res;
        }

        public FullAccountServerData GetFullProfile(string login)
        {
            var user = GetProfile(login);
            var data = new FullAccountServerData(user);
            data.History = UserContext.Data.GameEvents
                .Where(x => x.User == user.Login)
                .OrderByDescending(x => x.Time).ToList();
            return data;
        }
    }
}