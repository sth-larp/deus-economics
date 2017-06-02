using RestSharp;
using System.Threading.Tasks;
using WispCloudClient.ApiTypes;

namespace WispCloudClient.ApiCommands
{
    public sealed class VoiceTurboModeOffCommand : BaseCommand
    {
        public override Method Method { get { return Method.POST; } }
        public override string Name { get { return "DEBUG: Make voice command to set Turbo mode OFF"; } }
        public override string Resource { get { return "api/debug/voice/turboOff/{HubSN}"; } }
        public override AccountRoles Roles
        {
            get
            {
                return AccountRoles.None;
            }
        }

        public async Task<CommandResponse> ExecuteAsync(CloudClient client, decimal hubSN)
        {
            var request = CreateRequest(client);
            request.AddUrlSegment("HubSN", hubSN.ToString());

            return await ExecuteRequestAsync(client, request);
        }

    }

}
