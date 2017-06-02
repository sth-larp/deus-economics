using RestSharp;
using System.Collections.Generic;
using System.Threading.Tasks;
using WispCloudClient.ApiTypes;

namespace WispCloudClient.ApiCommands
{
    class GetUnassignedButtonWallSwithcesCommand : OutputCommand<List<string>>
    {
        public override Method Method { get { return Method.GET; } }
        public override string Name { get { return "Get unassigned button wall swithces"; } }
        public override string Resource { get { return "api/bws"; } }
        public override AccountRoles Roles { get { return AccountRoles.SeviceEnginier | AccountRoles.User; } }

        public async Task<CommandResponse<List<string>>> ExecuteAsync(CloudClient client, long installationID)
        {
            var request = CreateRequest(client);
            request.AddUrlSegment("InstallationID", installationID.ToString());

            return await this.ExecuteRequestExactAsync(client, request);
        }

    }

}
