using RestSharp;
using System.Threading.Tasks;
using WispCloudClient.ApiTypes;

namespace WispCloudClient.ApiCommands
{
    public sealed class CreateHubCommand : InputOutputCommand<string, HubCreateClientData>
    {
        public override Method Method { get { return Method.POST; } }
        public override string Name { get { return "Create hub"; } }
        public override string Resource { get { return "api/hubs"; } }
        public override AccountRoles Roles { get { return AccountRoles.SeviceEnginier | AccountRoles.User; } }

        protected override object GetRequestBodyTemplate()
        {
            return "1";
        }

        public async Task<CommandResponse<HubCreateClientData>> ExecuteAsync(CloudClient client, long installationID, string hubSN)
        {
            var request = CreateRequest(client);
            request.AddUrlSegment("InstallationID", installationID.ToString());
            request.AddJsonBody(hubSN);

            return await this.ExecuteRequestExactAsync(client, request);
        }

    }

}
