﻿using System.Collections.Generic;
using System.Linq;
using DeusCloud.Data.Entities;
using DeusCloud.Data.Entities.Access;
using DeusCloud.Data.Entities.Accounts;
using DeusCloud.Exceptions;
using DeusCloud.Identity;
using DeusCloud.Logic.CommonBase;
using DeusCloud.Logic.Rights.Client;
using Microsoft.AspNet.Identity;

namespace DeusCloud.Logic.Rights
{
    public sealed class RightsManager : ContextHolder
    {
        private UserManager _userManager;
        static string NotEnoughRightsMessageText { get; }

        static string NotEnoughPrivilegeText { get; }
        static string UserBlockedMessageText { get; }

        static RightsManager()
        {
            NotEnoughPrivilegeText = "User did not allow this level of access;";
            NotEnoughRightsMessageText = "You are not allowed to perform this operation;";
            UserBlockedMessageText = "Your account is blocked;";
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

        public void CheckForAccessOverSlave(string slave, AccountAccessRoles roles)
        {
            CheckCurrentUserActive();
            var slaveAccount = _userManager.FindById(slave);
            Try.NotNull(slaveAccount, $"Cant find account with login: {slave}.");
            
            //Admin can do anything
            if ((UserContext.CurrentUser.Role & AccountRole.Admin) > 0)
                return;

            //You have all access rights for yourself
            if (UserContext.CurrentUser.Login == slave)
                return;

            var accessLevel = GetCurrentAccountAccess(slave);
            Try.Condition(accessLevel != null && (accessLevel.Role & roles) > 0, NotEnoughPrivilegeText);
        }


        public Account SetAccountProperties(AccPropertyClientData clientData)
        {
            Try.Argument(clientData, nameof(clientData));
            CheckRole(AccountRole.Admin);
                
            var roles = clientData.Roles?.Aggregate(AccountRole.None, (curr, r) => curr |= r);

            var editAccount = UserContext.Accounts.Get(clientData.Login);
            Try.NotNull(editAccount, $"Cant find account with login: {clientData.Login}.");

            if (roles != null)
                editAccount.Role = roles.Value;

            editAccount.Status = clientData.Status;

            UserContext.Accounts.Update(editAccount);
            return editAccount;
        }

        public AccountAccess SetAccountAccess(AccountAccessClientData accessData)
        {
            CheckForAccessOverSlave(accessData.SlaveLogin, AccountAccessRoles.Admin);
            Try.Argument(accessData, nameof(accessData));

            var slaveAccount = _userManager.FindById(accessData.SlaveLogin);
            var masterAccount = _userManager.FindById(accessData.MasterLogin);
            Try.NotNull(masterAccount, $"Cant find account with login: {accessData.MasterLogin}.");

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