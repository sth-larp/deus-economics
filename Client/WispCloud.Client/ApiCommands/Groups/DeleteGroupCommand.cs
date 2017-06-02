using RestSharp;
using System.Threading.Tasks;
using WispCloudClient.ApiTypes;

namespace WispCloudClient.ApiCommands
{
    class DeleteGroupCommand : BaseCommand
    {
        public override Method Method { get { return Method.DELETE; } }
        public override string Name { get { return "Delete group"; } }
        public override AccountRoles Roles { get { return AccountRoles.SeviceEnginier | AccountRoles.User; } }

        public override string Resource
        {
            get { return "api/installations/{InstallationID}/groups/{GroupID}"; }
        }

        public async Task<CommandResponse> ExecuteAsync(CloudClient client, long installationID, int groupId)
        {
            var request = CreateRequest(client);
            request.AddUrlSegment("InstallationID", installationID.ToString());
            request.AddUrlSegment("GroupID", groupId.ToString());

            return await ExecuteRequestAsync(client, request);
        }

    }

}
