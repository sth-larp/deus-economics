using RestSharp;
using System.Globalization;
using System.Threading.Tasks;
using WispCloudClient.ApiTypes;

namespace WispCloudClient.ApiCommands
{
    public sealed class UpdateHubSettingsCommand : InputCommand<HubSettingsClientData>
    {
        public override Method Method { get { return Method.POST; } }
        public override string Name { get { return "Update hub settings"; } }
        public override string Resource { get { return "api/hubs/{hubSN}/settings"; } }
        public override AccountRoles Roles { get { return AccountRoles.Hub | AccountRoles.SeviceEnginier | AccountRoles.Installer; } }

        protected override object GetRequestBodyTemplate()
        {
            return new HubSettingsClientData()
            {
                HubSN = "1234567890123456789012345",
                HeartbeatPeriod = 10000
            };
        }

        protected override object GenerateBodyRequest()
        {
            return new HubSettingsClientData()
            {
                HubSN = "1234567890123456789012345",
                HeartbeatPeriod = 10000
            };
        }

        public async Task<CommandResponse> ExecuteAsync(CloudClient client, string hubSN, HubSettingsClientData clientData)
        {
            var request = CreateRequest(client);
            request.AddUrlSegment("hubSN", hubSN);
            request.AddJsonBody(clientData);

            return await ExecuteRequestAsync(client, request);
        }

    }

}
