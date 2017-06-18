using System.Collections.Generic;
using System.Linq;
using DeusCloud.Data.Entities.Access;
using DeusCloud.Data.Entities.Accounts;
using DeusCloud.Exceptions;
using DeusCloud.Identity;
using DeusCloud.Logic.Client;
using DeusCloud.Logic.CommonBase;
using Microsoft.AspNet.Identity;

namespace DeusCloud.Logic.Managers
{
    public sealed class RightsManager : ContextHolder
    {
        private UserManager _userManager;
        static string NotEnoughRightsMessageText { get; }

        static string NotEnoughPrivilegeText { get; }
        static string UserBlockedMessageText { get; }

        static RightsManager()
        {
            NotEnoughPrivilegeText = "Недостаточно прав на аккаунт данного пользователя;";
            NotEnoughRightsMessageText = "Недостаточно прав;";
            UserBlockedMessageText = "Ваш аккаунт заблокирован;";
        }

        public RightsManager(UserContext context)
            : base(context)
        {
            _userManager = new UserManager(UserContext);
        }

        public void CheckCurrentUserActive()
        {
            Try.NotNull(UserContext.CurrentUser, NotEnoughRightsMessageText);
            Try.Condition(UserContext.CurrentUser.Status == AccountStatus.Active, UserBlockedMessageText);
        }

        public void CheckRole(AccountRole role)
        {
            CheckCurrentUserActive();
            Try.Condition((UserContext.CurrentUser.Role & role) > 0, NotEnoughRightsMessageText);
        }

        AccountAccess GetCurrentAccountAccess(string slave)
        {
            return UserContext.Data.AccountAccesses.Find(slave, UserContext.CurrentUser.Login);
        }

        public Account CheckForAccessOverSlave(string slave, AccountAccessRoles roles)
        {
            CheckCurrentUserActive();
            var slaveAccount = _userManager.FindById(slave);
            Try.NotNull(slaveAccount, $"Не найден логин: {slave}.");
            
            //Admin can do anything
            if ((UserContext.CurrentUser.Role & AccountRole.Admin) > 0)
                return slaveAccount;

            //You have all access rights for yourself
            if (UserContext.CurrentUser.Login == slave)
                return slaveAccount;

            var accessLevel = GetCurrentAccountAccess(slave);
            Try.Condition(accessLevel != null && (accessLevel.Role & roles) > 0, NotEnoughPrivilegeText);
            return slaveAccount;
        }


        public Account SetAccountProperties(AccPropertyClientData clientData)
        {
            Try.Argument(clientData, nameof(clientData));
            CheckRole(AccountRole.Admin);
                
            var roles = clientData.Roles?.Aggregate(AccountRole.None, (curr, r) => curr |= r);

            var editAccount = _userManager.FindById(clientData.Login);
            Try.NotNull(editAccount, $"Не найден логин: {clientData.Login}.");

            if (roles != null)
                editAccount.Role = roles.Value;

            if(clientData.Status != null)
                editAccount.Status = clientData.Status.Value;

            if (clientData.Insurance != null)
            {
                Try.Condition(clientData.Insurance == InsuranceType.None || (editAccount.Role & AccountRole.Person) > 0, 
                    $"Страховку можно дать только персоне: {clientData.Login}.");
                Try.Condition(clientData.InsuranceLevel != null, $"Не задан уровень страховки");

                editAccount.Insurance = clientData.Insurance.Value;
            }

            if (clientData.InsuranceLevel != null)
            {
                Try.Condition((IsCorporate(editAccount.Insurance) && clientData.InsuranceLevel <= 3) 
                    || (editAccount.Insurance == InsuranceType.Govt && clientData.InsuranceLevel <= 2)
                    || clientData.InsuranceLevel == 1,
                    $"Неверное значение уровня. 1-3 для корпораций, 1-2 для правительства, 1 в прочих случаях: {clientData.InsuranceLevel}");
                editAccount.InsuranceLevel = clientData.InsuranceLevel.Value;
            }

            UserContext.Accounts.Update(editAccount);
            return editAccount;
        }

        public Account SetAccountIndex(AccIndexClientData data)
        {
            CheckRole(AccountRole.Admin);

            var editAccount = _userManager.FindById(data.Login);
            Try.NotNull(editAccount, $"Не найден логин: {data.Login}.");

            editAccount.Index = data.Index;
            editAccount.IndexSpent = data.IndexSpent;
            UserContext.Accounts.Update(editAccount);
            return editAccount;
        }

        private bool IsCorporate(InsuranceType insurance)
        {
            return insurance == InsuranceType.Panam
                   || insurance == InsuranceType.JJ
                   || insurance == InsuranceType.Serenity;
        }

        public AccountAccess SetAccountAccess(AccountAccessClientData accessData)
        {
            CheckForAccessOverSlave(accessData.SlaveLogin, AccountAccessRoles.Admin);
            Try.Argument(accessData, nameof(accessData));

            var slaveAccount = _userManager.FindById(accessData.SlaveLogin);
            var masterAccount = _userManager.FindById(accessData.MasterLogin);
            Try.NotNull(masterAccount, $"Не найден логин: {accessData.MasterLogin}.");

            var newRole = accessData.Roles.Aggregate(AccountAccessRoles.None, (role, next) => role |= next);

            var currentAccess = UserContext.Data.AccountAccesses.
                Find(accessData.SlaveLogin, accessData.MasterLogin);

            if (currentAccess == null)
                currentAccess = CreateAccountAccess(slaveAccount, masterAccount, newRole);
            else
                currentAccess.Role = newRole;

            UserContext.Data.SaveChanges();

            return currentAccess;
        }

        public AccountAccess CreateAccountAccess(Account slave, Account master, AccountAccessRoles roles)
        {
            var access = new AccountAccess(slave, master, roles);
            UserContext.Data.AccountAccesses.Add(access);
            return access;
        }

        public List<AccountAccess> GetAccessMasters(string slave)
        {
            CheckForAccessOverSlave(slave, AccountAccessRoles.Read);

            return UserContext.Data.AccountAccesses.Where(x => x.Slave == slave).ToList();
        }

        public List<AccountAccess> GetAccessSlaves(string master)
        {
            CheckForAccessOverSlave(master, AccountAccessRoles.Read);

            return UserContext.Data.AccountAccesses.Where(x => x.Master == master).ToList();
        }
    }

}