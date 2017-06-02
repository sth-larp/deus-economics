using System.Linq;
using WispCloud.Data;

namespace WispCloud.Logic
{
    public sealed class RightsManager : BaseManager
    {
        const string NotEnoughRightsMessageText = "You are not allowed to perform this operation;";
        const string InstallationIsDisabledMessageText = "Cant find installation;";

        public RightsManager(WispContext context)
            : base(context)
        {
        }

        public void CheckUser()
        {
            Try.NotNull(Context.CurrentUser, NotEnoughRightsMessageText);
            Try.Condition(Context.CurrentUser.Active, NotEnoughRightsMessageText);
        }

        public void CheckRole(AccountRoles role)
        {
            CheckUser();
            Try.Condition((role & Context.CurrentUser.Role) > 0, NotEnoughRightsMessageText);
        }

        InstallationAccess GetInstallationAcces(long installationID)
        {
            return Context.Data.InstallationAccesses
                .FirstOrDefault(x => x.InstallationID == installationID && x.Login == Context.CurrentUser.Login);
        }

        void CheckInstallationExistsAndActive(long installationID)
        {
            var installation = Context.Data.Installations.Find(installationID);
            Try.NotNull(installation, InstallationIsDisabledMessageText);
        }

        public void CheckInstallationAceesible(long installationID)
        {
            CheckUser();
            CheckInstallationExistsAndActive(installationID);
            var installationAcces = GetInstallationAcces(installationID);
            Try.NotNull(installationAcces, NotEnoughRightsMessageText);
        }

        public void CheckRoleInInstallation(long installationID, InInstallationRoles role)
        {
            CheckUser();
            CheckInstallationExistsAndActive(installationID);
            var installationAcces = GetInstallationAcces(installationID);
            Try.Condition(installationAcces != null && (role & installationAcces.Role) > 0, NotEnoughRightsMessageText);
        }

        public void CheckRoles(AccountRoles accountRole, long installationID, InInstallationRoles inInstallationRole)
        {
            CheckRole(accountRole);
            CheckRoleInInstallation(installationID, inInstallationRole);
        }

    }

}