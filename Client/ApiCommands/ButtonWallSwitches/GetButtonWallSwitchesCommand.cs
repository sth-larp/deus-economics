using RestSharp;
using System.Collections.Generic;
using System.Threading.Tasks;
using WispCloudClient.ApiTypes;

namespace WispCloudClient.ApiCommands
{
    public sealed class GetButtonWallSwitchesCommand : OutputCommand<List<ButtonWallSwitch>>
    {
        public override Method Method { get { return Method.GET; } }
        public override string Name { get { return "Get button wall switches"; } }
        public override string Resource { get { return "api/installations/{InstallationID}/bws"; } }
        public override AccountRoles Roles
        {
            get { return AccountRoles.SeviceEnginier | AccountRoles.User | AccountRoles.Hub; }
        }

        public async Task<CommandResponse<List<ButtonWallSwitch>>> ExecuteAsync(CloudClient client, long installationID)
        {
            var request = CreateRequest(client);
            request.AddUrlSegment("InstallationID", installationID.ToString());

            return await this.ExecuteRequestExactAsync(client, request);
        }

    }

}
