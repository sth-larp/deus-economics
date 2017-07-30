using System;
using System.Collections.Generic;
using System.Linq;
using DeusCloud.Data.Entities.Access;
using DeusCloud.Data.Entities.Accounts;
using DeusCloud.Data.Entities.GameEvents;
using DeusCloud.Exceptions;
using DeusCloud.Identity;
using DeusCloud.Logic.Client;
using DeusCloud.Logic.CommonBase;
using Microsoft.AspNet.Identity;

namespace DeusCloud.Logic.Managers
{
    public sealed class RightsManager : ContextHolder
    {
        static string NotEnoughRightsMessageText { get; }

        static string NotEnoughPrivilegeText { get; }
        static string UserBlockedMessageText { get; }

        static RightsManager()
        {
            NotEnoughPrivilegeText = "Недостаточно прав на аккаунт данного пользователя;";
            NotEnoughRightsMessageText = "Недостаточно прав для данного запроса;";
            UserBlockedMessageText = "Ваш аккаунт заблокирован;";
        }

        public RightsManager(UserContext context): base(context)
        {
        }

        public void CheckRole(AccountRole role)
        {
            Try.Condition((UserContext.CurrentUser.Role & role) > 0, NotEnoughRightsMessageText);
        }

        AccountAccess GetCurrentAccountAccess(string slave)
        {
            return UserContext.Data.AccountAccesses.Find(slave, UserContext.CurrentUser.Login);
        }

        public Account CheckForAccessOverSlave(string slave, AccountAccessRoles roles)
        {
            var slaveAccount = UserContext.Accounts.GetOrFail(slave); //_userManager.FindById(slave);
           
            //Admin can do anything
            return CheckForAccessOverSlave(slaveAccount, roles);
        }

        public Account CheckForAccessOverSlave(Account slaveAccount, AccountAccessRoles role)
        {
            //Admin can do anything
            if (UserContext.CurrentUser.Role == AccountRole.Admin)
                return slaveAccount;

            //Master can read anything
            if (UserContext.CurrentUser.Role == AccountRole.Master && role == AccountAccessRoles.Read)
                return slaveAccount;

            //You have all access rights for yourself
            if (UserContext.CurrentUser.Login == slaveAccount.Login)
                return slaveAccount;

            var accessLevel = GetCurrentAccountAccess(slaveAccount.Login);
            Try.Condition(accessLevel != null && accessLevel.Role >= role, NotEnoughPrivilegeText);
            return slaveAccount;
        }

        public Account SetAccountIndex(AccIndexClientData data)
        {
            CheckRole(AccountRole.Admin);

            var editAccount = UserContext.Accounts.GetOrFail(data.Login);

            editAccount.Index = data.Index;
            editAccount.InsurancePoints = data.InsurancePoints;
            UserContext.Accounts.Update(editAccount);

            UserContext.AddGameEvent(editAccount.Login, GameEventType.Index, 
                $"Задан новый индекс {editAccount.Index} и очки страховки {editAccount.InsurancePoints}");

            return editAccount;
        }

        public AccountAccess SetAccountAccess(AccountAccessClientData accessData)
        {
            Try.Argument(accessData, nameof(accessData));

            var slaveAccount = UserContext.Accounts.GetOrFail(accessData.SlaveLogin);
            var masterAccount = UserContext.Accounts.GetOrFail(accessData.MasterLogin);

            if (accessData.Role != AccountAccessRoles.None)
                CheckForAccessOverSlave(slaveAccount, AccountAccessRoles.Admin);

            return SetAccountAccess_Checked(slaveAccount, masterAccount, accessData.Role);
        }

        public AccountAccess SetAccountAccess_Checked(Account slave, Account master, AccountAccessRoles role)
        {
            var currentAccess = UserContext.Data.AccountAccesses.Find(slave.Login, master.Login);

            if (role == AccountAccessRoles.None && currentAccess == null)
                return null;

            if (role == AccountAccessRoles.None)
            {
                UserContext.Data.AccountAccesses.Remove(currentAccess);
                currentAccess = null;
            }
            else if (currentAccess == null)
            {
                currentAccess = new AccountAccess(slave, master, role);
                UserContext.Data.AccountAccesses.Add(currentAccess);
            }
            else
                currentAccess.Role = role;

            UserContext.Data.SaveChanges();
            SetAccessChangeGameEvents(slave, master, role);
            return currentAccess;
        }

        public void SetAccessChangeGameEvents(Account slave, Account master, AccountAccessRoles role)
        {
            UserContext.AddGameEvent(slave.Login, GameEventType.Rights,
                $"Доступ {master.DisplayName} к вашему счету изменен на {role}");

            UserContext.AddGameEvent(master.Login, GameEventType.Rights,
                $"Доступ над счетом {slave.DisplayName} изменен на {role}");
        }

        public List<AccountAccess> GetAccessMasters(string slave)
        {
            var acc = CheckForAccessOverSlave(slave, AccountAccessRoles.Read);

            return UserContext.Data.AccountAccesses.Where(x => x.Slave == acc.Login).ToList();
        }

        public List<AccountAccess> GetAccessSlaves(string master)
        {
            var acc = CheckForAccessOverSlave(master, AccountAccessRoles.Read);

            if(acc.Role != AccountRole.Admin && acc.Role != AccountRole.Master)
                return UserContext.Data.AccountAccesses.Where(x => x.Master == acc.Login).ToList();

            //Говнокод
            var companies = UserContext.Data.Accounts.Where(x => x.Role == AccountRole.Company
                                                                 || x.Role == AccountRole.Corp
                                                                 || x.Role == AccountRole.Govt).ToList();

            var accesses = companies.Select(x => new AccountAccess(x, acc)
            {
                Role = acc.Role == AccountRole.Admin? AccountAccessRoles.Admin : AccountAccessRoles.Read
            }).OrderBy(x => x.SlaveAccount.Role).ToList();
            return accesses;
        }
    }

}