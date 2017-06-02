using RestSharp;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using WispCloudClient.ApiTypes;

namespace WispCloudClient.ApiCommands
{
    public sealed class GetHubSettingsCommand : OutputCommand<HubSettingsClientData>
    {
        public override Method Method { get { return Method.GET; } }
        public override string Name { get { return "Get hub settings"; } }
        public override string Resource { get { return "api/hubs/{hubSN}/settings"; } }
        public override AccountRoles Roles
        {
            get { return AccountRoles.SeviceEnginier | AccountRoles.User | AccountRoles.Hub; }
        }

        public async Task<CommandResponse<HubSettingsClientData>> ExecuteAsync(CloudClient client, string hubSN)
        {
            var request = CreateRequest(client);
            request.AddUrlSegment("hubSN", hubSN);

            return await this.ExecuteRequestExactAsync(client, request);
        }

    }

}
