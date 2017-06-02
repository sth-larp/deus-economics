using RestSharp;
using System.Threading.Tasks;
using WispCloudClient.ApiTypes;

namespace WispCloudClient.ApiCommands
{
    public sealed class GetPlanCommand : OutputCommand<PlanView>
    {
        public override Method Method { get { return Method.GET; } }
        public override string Name { get { return "Get plan"; } }
        public override string Resource { get { return "api/installations/{InstallationID}/plan"; } }
        public override AccountRoles Roles
        {
            get { return AccountRoles.SeviceEnginier | AccountRoles.User | AccountRoles.Hub; }
        }

        public async Task<CommandResponse<PlanView>> ExecuteAsync(CloudClient client, long installationID)
        {
            var request = CreateRequest(client);
            request.AddUrlSegment("InstallationID", installationID.ToString());

            return await this.ExecuteRequestExactAsync(client, request);
        }

    }

}
