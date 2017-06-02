using RestSharp;
using System.Collections.Generic;
using System.Threading.Tasks;
using WispCloudClient.ApiTypes;

namespace WispCloudClient.ApiCommands
{
    public sealed class GetInstallationsCommand : OutputCommand<List<InstallationClientData>>
    {
        public override Method Method { get { return Method.GET; } }
        public override string Name { get { return "Get installations"; } }
        public override string Resource { get { return "api/installations"; } }
        public override AccountRoles Roles
        {
            get { return AccountRoles.SeviceEnginier | AccountRoles.User | AccountRoles.Hub; }
        }

        public async Task<CommandResponse<List<InstallationClientData>>> ExecuteAsync(CloudClient client)
        {
            var request = CreateRequest(client);

            return await this.ExecuteRequestExactAsync(client, request);
        }

    }

}
