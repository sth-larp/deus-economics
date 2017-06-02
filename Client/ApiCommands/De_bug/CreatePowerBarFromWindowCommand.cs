using RestSharp;
using System.Threading.Tasks;
using WispCloudClient.ApiTypes;

namespace WispCloudClient.ApiCommands
{
    public sealed class CreatePowerBarFromWindowCommand : OutputCommand<PowerBar>
    {
        public override Method Method { get { return Method.GET; } }
        public override string Name { get { return "DEBUG: Create power bar from windows"; } }
        public override string Resource { get { return "api/debug/installations/{InstallationID}/window/{WindowID}/addpowerbar"; } }
        public override AccountRoles Roles
        {
            get { return AccountRoles.SeviceEnginier | AccountRoles.User | AccountRoles.Hub; }
        }
        public override int SortIndex { get { return 10; } }

        public async Task<CommandResponse<PowerBar>> ExecuteAsync(CloudClient client, long installationID, int windowID)
        {
            var request = CreateRequest(client);
            request.AddUrlSegment("InstallationID", installationID.ToString());
            request.AddUrlSegment("WindowID", windowID.ToString());

            return await this.ExecuteRequestExactAsync(client, request);
        }

    }

}
