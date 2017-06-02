using RestSharp;
using System.Threading.Tasks;
using WispCloudClient.ApiTypes;

namespace WispCloudClient.ApiCommands
{
    public sealed class DeleteTimerCommand : BaseCommand
    {
        public override Method Method { get { return Method.DELETE; } }
        public override string Name { get { return "Delete timer"; } }
        public override string Resource { get { return "api/installations/{InstallationID}/modes/timers/{TimerID}"; } }
        public override AccountRoles Roles { get { return AccountRoles.SeviceEnginier | AccountRoles.User; } }

        public async Task<CommandResponse> ExecuteAsync(CloudClient client, long installationID, int timerID)
        {
            var request = CreateRequest(client);
            request.AddUrlSegment("InstallationID", installationID.ToString());
            request.AddUrlSegment("TimerID", timerID.ToString());

            return await ExecuteRequestAsync(client, request);
        }

    }

}
