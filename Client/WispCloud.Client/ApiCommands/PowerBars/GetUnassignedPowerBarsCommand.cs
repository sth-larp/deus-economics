using RestSharp;
using System.Collections.Generic;
using System.Threading.Tasks;
using WispCloudClient.ApiTypes;

namespace WispCloudClient.ApiCommands
{
    public sealed class GetUnassignedPowerBarsCommand : OutputCommand<List<PowerBar>>
    {
        public override Method Method { get { return Method.GET; } }
        public override string Name { get { return "Get unassigned power bars"; } }
        public override string Resource { get { return "api/powerbars"; } }
        public override AccountRoles Roles { get { return AccountRoles.SeviceEnginier | AccountRoles.User; } }

        public async Task<CommandResponse<List<PowerBar>>> ExecuteAsync(CloudClient client)
        {
            var request = CreateRequest(client);
            return await this.ExecuteRequestExactAsync(client, request);
        }

    }

}
