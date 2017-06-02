using RestSharp;
using System.Threading.Tasks;
using WispCloudClient.ApiTypes;

namespace WispCloudClient.ApiCommands
{
    public sealed class DeletePowerBarCommand : BaseCommand
    {
        public override Method Method { get { return Method.DELETE; } }
        public override string Name { get { return "Delete power bar"; } }
        public override string Resource { get { return "api/powerbars/{PowerBarSN}"; } }
        public override AccountRoles Roles { get { return AccountRoles.SeviceEnginier | AccountRoles.User; } }

        public async Task<CommandResponse> ExecuteAsync(CloudClient client, string powerBarSN)
        {
            var request = CreateRequest(client);
            request.AddUrlSegment("PowerBarSN", powerBarSN);

            return await ExecuteRequestAsync(client, request);
        }

    }

}
