using RestSharp;
using System.Threading.Tasks;
using WispCloudClient.ApiTypes;

namespace WispCloudClient.ApiCommands
{
    public sealed class DeleteModeCommand : BaseCommand
    {
        public override Method Method { get { return Method.DELETE; } }
        public override string Name { get { return "Delete timer mode"; } }
        public override string Resource { get { return "api/installations/{InstallationID}/modes/{ModeID}"; } }
        public override AccountRoles Roles { get { return AccountRoles.SeviceEnginier | AccountRoles.User; } }

        public async Task<CommandResponse> ExecuteAsync(CloudClient client, long installationID, int modeID)
        {
            var request = CreateRequest(client);
            request.AddUrlSegment("InstallationID", installationID.ToString());
            request.AddUrlSegment("ModeID", modeID.ToString());

            return await ExecuteRequestAsync(client, request);
        }

    }

}
