using RestSharp;
using System.Threading.Tasks;
using WispCloudClient.ApiTypes;

namespace WispCloudClient.ApiCommands
{
    public sealed class GetModesCommand : OutputCommand<ModesClientData>
    {
        public override Method Method { get { return Method.GET; } }
        public override string Name { get { return "Get modes"; } }
        public override string Resource { get { return "api/installations/{InstallationID}/modes"; } }
        public override AccountRoles Roles
        {
            get { return AccountRoles.SeviceEnginier | AccountRoles.User | AccountRoles.Hub; }
        }

        public async Task<CommandResponse<ModesClientData>> ExecuteAsync(CloudClient client, long installationID)
        {
            var request = CreateRequest(client);
            request.AddUrlSegment("InstallationID", installationID.ToString());

            return await this.ExecuteRequestExactAsync(client, request);
        }

    }

}
