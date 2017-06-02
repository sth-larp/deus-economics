using System;
using System.Linq;
using DeusCloud.Data.Entities;
using DeusCloud.Data.Entities.Accounts;
using DeusCloud.Exceptions;
using DeusCloud.Logic;
using DeusCloud.Logic.CommonBase;
using DeusCloud.Logic.Rights.Client;
using WispCloud.Data;

namespace WispCloud.Logic
{
    public sealed class RightsManager : ContextHolder
    {
        static string NotEnoughRightsMessageText { get; }

        static string UserBlockedMessageText { get; }
        static string InstallationIsDisabledMessageText { get; }

        static RightsManager()
        {
            NotEnoughRightsMessageText = "You are not allowed to perform this operation;";
            UserBlockedMessageText = "Your account is blocked;";
            InstallationIsDisabledMessageText = "Cant find installation;";
        }

        public RightsManager(UserContext context)
            : base(context)
        {
        }

        public void CheckUser()
        {
            Try.NotNull(UserContext.CurrentUser, NotEnoughRightsMessageText);
            Try.Condition(UserContext.CurrentUser.Status == AccountStatus.Active, 
                NotEnoughRightsMessageText);
        }

        public void CheckUserExistsAndActive(string user)
        {
            Try.NotNull(UserContext.CurrentUser, NotEnoughRightsMessageText);
            Try.Condition(UserContext.CurrentUser.Status == AccountStatus.Active,
                NotEnoughRightsMessageText);
        }

        public void CheckRole(AccountRole role)
        {
            CheckUser();
            Try.Condition((UserContext.CurrentUser.Role & role) > 0, NotEnoughRightsMessageText);
        }

        AccountAccess GetCurrentAccountAccess(string slave)
        {
            return UserContext.Data.AccountAccesses.Find(slave, UserContext.CurrentUser.Login);
        }

        public void CheckForOperation(string slave, AccountAccessRoles role)
        {
            CheckUser();
            var account = _userManager.FindById(slave);

            //Master can do anything
            if ((UserContext.CurrentUser.Role & AccountRole.Master) > 0)
                return;

            var installationAccess = GetCurrentAccountAccess(slave);
            Try.Condition(installationAccess != null && (installationAccess.Role & role) > 0, NotEnoughRightsMessageText);
        }


        public void SetProperties(AccPropertyClientData clientData)
        {
            Try.Argument(clientData, nameof(clientData));

            AccountRole? role = null;
            if (clientData.Roles != null)
                role = clientData.Roles.Aggregate(AccountRole.None, (aggregatedRole, next) => aggregatedRole |= next);

            SetProperties(clientData.Login, role, clientData.Active);
        }

        public void SetProperties(string login, AccountRole? roles, bool? active)
        {
            CheckRole(AccountRole.SeviceEnginier);

            if (roles.HasValue) 
                Try.Condition((roles.Value & AccountRole.Hub) == 0,
                    "Cant assign Hub role, to create hubs use Hub api.");

            var account = UserContext.Accounts.Get(login);
            Try.NotNull(account, $"Cant find account with login: {login}.");

            if (roles.HasValue)
            {
                Try.Condition(account.Role != AccountRole.Hub, "Cant change role for hub.");
                account.Role = roles.Value;
            }
            if (active.HasValue)
                account.Active = active.Value;

            UserContext.Accounts.Update(account);
        }

        public void SetAccountAccesses(long installationID, AccountAccessClientData clientData)
        {
            Try.Argument(clientData, nameof(clientData));
            SetAccountAccesses(installationID, clientData.MasterLogin,
                clientData.Roles.Aggregate(AccountAccessRoles.None, (role, next) => role |= next));
        }

    }

}