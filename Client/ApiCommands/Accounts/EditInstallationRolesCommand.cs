using RestSharp;
using System.Collections.Generic;
using System.Threading.Tasks;
using WispCloudClient.ApiTypes;

namespace WispCloudClient.ApiCommands
{
    public sealed class EditInstallationRolesCommand : InputCommand<InstallationRolesClientData>
    {
        public override Method Method { get { return Method.POST; } }
        public override string Name { get { return "Edit account roles in installation"; } }
        public override string Resource { get { return "api/installations/{InstallationID}/accounts/roles"; } }
        public override AccountRoles Roles
        {
            get { return AccountRoles.SeviceEnginier | AccountRoles.User | AccountRoles.Hub; }
        }

        protected override object GetRequestBodyTemplate()
        {
            return new InstallationRolesClientData()
            {
                Login = "email@example.com",
                Roles = new List<InstallationAccessRoles>()
                {
                    InstallationAccessRoles.User,
                    InstallationAccessRoles.Administrator
                },

            };
        }

        public async Task<CommandResponse> ExecuteAsync(CloudClient client, long installationID, InstallationRolesClientData clientData)
        {
            var request = CreateRequest(client);
            request.AddUrlSegment("InstallationID", installationID.ToString());
            request.AddJsonBody(clientData);

            return await this.ExecuteRequestAsync(client, request);
        }

    }

}
