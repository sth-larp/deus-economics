﻿using System;
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
        private UserManager _userManager;
        static string NotEnoughRightsMessageText { get; }

        static string NotEnoughPrivilegeText { get; }
        static string UserBlockedMessageText { get; }

        static RightsManager()
        {
            NotEnoughPrivilegeText = "Недостаточно прав на аккаунт данного пользователя;";
            NotEnoughRightsMessageText = "Недостаточно прав для данного запроса;";
            UserBlockedMessageText = "Ваш аккаунт заблокирован;";
        }

        public RightsManager(UserContext context)
            : base(context)
        {
            _userManager = new UserManager(UserContext);
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
            var slaveAccount = _userManager.FindById(slave);
            Try.NotNull(slaveAccount, $"Не найден логин: {slave}.");
            
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

        public Account SetAccountProperties(AccPropertyClientData clientData)
        {
            Try.Argument(clientData, nameof(clientData));
            CheckRole(AccountRole.Admin);
                
            var editAccount = _userManager.FindById(clientData.Login);
            Try.NotNull(editAccount, $"Не найден логин: {clientData.Login}.");

            if (clientData.Role != null)
            {
                editAccount.Role = clientData.Role.Value;
                UserContext.AddGameEvent(editAccount.Login, GameEventType.Rights, $"Изменен тип аккаунта");
            }

            if (clientData.Status != null)
                editAccount.Status = clientData.Status.Value;

            if (!String.IsNullOrEmpty(clientData.Fullname))
                editAccount.Fullname = clientData.Fullname;

            if (!String.IsNullOrEmpty(clientData.Email))
                editAccount.Email = clientData.Email;

            //if (clientData.Insurance != null)
            //{
            //    Try.Condition(clientData.Insurance == InsuranceType.None || (editAccount.Role & AccountRole.Person) > 0, 
            //        $"Страховку можно дать только персоне: {clientData.Login}.");
            //    Try.Condition(clientData.InsuranceLevel != null, $"Не задан уровень страховки");
            //
            //    editAccount.Insurance = clientData.Insurance.Value;
            //}

            //if (clientData.InsuranceLevel != null)
            //{
            //    Try.Condition((IsCorporate(editAccount.Insurance) && clientData.InsuranceLevel <= 3) 
            //        || (editAccount.Insurance == InsuranceType.Govt && clientData.InsuranceLevel <= 2)
            //        || clientData.InsuranceLevel == 1,
            //        $"Неверное значение уровня. 1-3 для корпораций, 1-2 для правительства, 1 в прочих случаях: {clientData.InsuranceLevel}");
            //    editAccount.InsuranceLevel = clientData.InsuranceLevel.Value;
            //}

            UserContext.Accounts.Update(editAccount);
            return editAccount;
        }

        public Account SetAccountIndex(AccIndexClientData data)
        {
            CheckRole(AccountRole.Admin);

            var editAccount = _userManager.FindById(data.Login);
            Try.NotNull(editAccount, $"Не найден логин: {data.Login}.");

            editAccount.Index = data.Index;
            editAccount.InsurancePoints = data.InsurancePoints;
            UserContext.Accounts.Update(editAccount);

            UserContext.AddGameEvent(editAccount.Login, GameEventType.Index, $"Задан новый индекс {data.Index}");

            return editAccount;
        }

        public AccountAccess SetAccountAccess(AccountAccessClientData accessData)
        {
            Try.Argument(accessData, nameof(accessData));

            var slaveAccount = _userManager.FindById(accessData.SlaveLogin);
            Try.NotNull(slaveAccount, $"Не найден логин: {accessData.SlaveLogin}.");

            if (accessData.Role != AccountAccessRoles.None)
                CheckForAccessOverSlave(slaveAccount, AccountAccessRoles.Admin);

            var masterAccount = _userManager.FindById(accessData.MasterLogin);
            Try.NotNull(masterAccount, $"Не найден логин: {accessData.MasterLogin}.");

            var currentAccess = UserContext.Data.AccountAccesses.
                Find(accessData.SlaveLogin, accessData.MasterLogin);

            if (accessData.Role == AccountAccessRoles.None && currentAccess == null)
                return null;

            if (accessData.Role == AccountAccessRoles.None)
            {
                UserContext.Data.AccountAccesses.Remove(currentAccess);
                currentAccess = null;
            }
            else if(currentAccess == null)
                currentAccess = CreateAccountAccess(slaveAccount, masterAccount, accessData.Role);
            else
                currentAccess.Role = accessData.Role;

            UserContext.Data.SaveChanges();
            SetAccessChangeGameEvents(slaveAccount, masterAccount);
            return currentAccess;
        }

        public AccountAccess CreateAccountAccess(Account slave, Account master, AccountAccessRoles roles)
        {
            var access = new AccountAccess(slave, master, roles);
            UserContext.Data.AccountAccesses.Add(access);
            SetAccessChangeGameEvents(slave, master);
            return access;
        }

        private void SetAccessChangeGameEvents(Account slave, Account master)
        {
            UserContext.AddGameEvent(slave.Login, GameEventType.Rights,
                $"Изменен доступ для пользователя {master.Login}");


            UserContext.AddGameEvent(master.Login, GameEventType.Rights,
                $"Изменен доступ над пользователем {slave.Login}");
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