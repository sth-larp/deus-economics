using RestSharp;
using System.Threading.Tasks;
using WispCloud.Logic;
using WispCloudClient.ApiTypes;

namespace WispCloudClient.ApiCommands
{
    public sealed class GetHubViewCommand : OutputCommand<HubViewClientData>
    {
        public override Method Method { get { return Method.GET; } }
        public override string Name { get { return "Get hub view"; } }
        public override string Resource { get { return "api/installations/{InstallationID}/views/hub"; } }
        public override AccountRoles Roles { get { return AccountRoles.Hub; } }

        public async Task<CommandResponse<HubViewClientData>> ExecuteAsync(CloudClient client, long installationID)
        {
            var request = CreateRequest(client);
            request.AddUrlSegment("InstallationID", installationID.ToString());

            return await this.ExecuteRequestExactAsync(client, request);
        }

    }

}
