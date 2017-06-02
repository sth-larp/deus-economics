using RestSharp;
using System.Threading.Tasks;
using WispCloudClient.ApiTypes;

namespace WispCloudClient.ApiCommands
{
    public sealed class NewHubPasswordCommand : OutputCommand<string>
    {
        public override Method Method { get { return Method.GET; } }
        public override string Name { get { return "DEBUG: generate new password for hub"; } }
        public override string Resource { get { return "api/debug/hubs/{HubSN}/newpassword"; } }
        public override AccountRoles Roles
        {
            get { return AccountRoles.SeviceEnginier | AccountRoles.User | AccountRoles.Hub; }
        }
        public override int SortIndex { get { return 10; } }

        public async Task<CommandResponse<string>> ExecuteAsync(CloudClient client, string hubSN)
        {
            var request = CreateRequest(client);
            request.AddUrlSegment("HubSN", hubSN);

            return await this.ExecuteRequestExactAsync(client, request);
        }

    }

}
