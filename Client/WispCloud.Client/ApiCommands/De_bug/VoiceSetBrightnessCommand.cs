using RestSharp;
using System.Threading.Tasks;
using WispCloudClient.ApiTypes;

namespace WispCloudClient.ApiCommands
{
    public sealed class VoiceSetBrightnessCommand : OutputCommand<PowerBar>
    {
        public override Method Method { get { return Method.POST; } }
        public override string Name { get { return "DEBUG: Make voice command to set window brightness and keep shading"; } }
        public override string Resource { get { return "api/debug/voice/brightness/{HubSN}/{brightness}"; } }
        public override AccountRoles Roles
        {
            get { return AccountRoles.None; }
        }
        public override int SortIndex { get { return 20; } }

        public async Task<CommandResponse<PowerBar>> ExecuteAsync(CloudClient client, string hubSN, int brightness)
        {
            var request = CreateRequest(client);
            request.AddUrlSegment("HubSN", hubSN);
            request.AddUrlSegment("brightness", brightness.ToString());

            return await this.ExecuteRequestExactAsync(client, request);
        }

    }

}
