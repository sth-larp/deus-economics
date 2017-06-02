using RestSharp;
using System.Threading.Tasks;
using WispCloudClient.ApiTypes;

namespace WispCloudClient.ApiCommands
{
    public class EditTimerCommand : InputOutputCommand<TimerClientData, Timer>
    {
        public override Method Method { get { return Method.PUT; } }
        public override string Name { get { return "Edit timer"; } }
        public override string Resource { get { return "api/installations/{InstallationID}/modes/timers/{TimerID}"; } }
        public override AccountRoles Roles { get { return AccountRoles.SeviceEnginier | AccountRoles.User; } }

        protected override object GetRequestBodyTemplate()
        {
            return new TimerClientData()
            {
                Name = "Edited timer",
                Hours = 18,
                Minutes = 10,
                UserBrightness = 50,
                Shading = 0,
            };
        }

        public async Task<CommandResponse<Timer>> ExecuteAsync(CloudClient client, long installationID, int timerID, TimerClientData clientData)
        {
            var request = CreateRequest(client);
            request.AddUrlSegment("InstallationID", installationID.ToString());
            request.AddUrlSegment("TimerID", timerID.ToString());
            request.AddJsonBody(clientData);

            return await this.ExecuteRequestExactAsync(client, request);
        }

    }

}
