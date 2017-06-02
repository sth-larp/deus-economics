using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DeusCloud.Data.Entities;
using DeusCloud.Data.Entities.Accounts;
using DeusCloud.Exceptions;
using DeusCloud.Logic.CommonBase;
using Microsoft.AspNet.Identity;
using WispCloud.Identity;
using WispCloud.Logic;

namespace DeusCloud.Logic.Transactions
{
    public sealed class TransactionsManager : ContextHolder
    {
        private UserManager _userManager;

        public TransactionsManager(UserContext context)
            : base(context)
        {
            _userManager = new UserManager(UserContext);
        }

        public InstallationClientData Create()
        {
            UserContext.Rights.CheckRole(AccountRole.SeviceEnginier | AccountRole.Installer);

            using (var transaction = UserContext.Data.Database.BeginTransaction())
            {
                UserContext.Data.BeginFastSave();

                var newInstallation = Installation.CreateWithNextID(UserContext.Data);
                UserContext.Data.Installations.Add(newInstallation);

                var access = UserContext.Rights.CreateInstallationAccess(
                    newInstallation, UserContext.CurrentUser,
                    AccountAccessRoles.Administrator);

                UserContext.Data.SaveChanges();
                transaction.Commit();

                UserContext.Events.InstallationChange(newInstallation.InstallationID, EventActionType.Create);

                return new InstallationClientData(newInstallation, UserContext.CurrentUser);
            }
        }

        public void Transfer(string login, string sender, string receiver, float amount)
        {
            var receiverAcc = _userManager.FindById(receiver);
            Try.NotNull(receiverAcc, $"Cant find account with login: {receiver}.");

            var senderAcc = _userManager.FindById(sender);
            Try.NotNull(senderAcc, $"Cant find account with login: {receiver}.");

            using (var transaction = UserContext.Data.Database.BeginTransaction())
            {
                UserContext.Data.BeginFastSave();

                var access = UserContext.Rights.CreateInstallationAccess(
                    newInstallation, UserContext.CurrentUser,
                    AccountAccessRoles.Administrator);

                UserContext.Data.SaveChanges();
                transaction.Commit();

                UserContext.Events.InstallationChange(newInstallation.InstallationID, EventActionType.Create);

                return new InstallationClientData(newInstallation, UserContext.CurrentUser);
            }
        }
    }
}