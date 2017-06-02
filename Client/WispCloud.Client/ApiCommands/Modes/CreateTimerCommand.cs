using RestSharp;
using System.Threading.Tasks;
using WispCloudClient.ApiTypes;

namespace WispCloudClient.ApiCommands
{
    public sealed class CreateTimerCommand : InputOutputCommand<TimerClientData, Timer>
    {
        public override Method Method { get { return Method.POST; } }
        public override string Name { get { return "Create timer"; } }
        public override string Resource { get { return "api/installations/{InstallationID}/modes/timers"; } }
        public override AccountRoles Roles { get { return AccountRoles.SeviceEnginier | AccountRoles.User; } }

        protected override object GetRequestBodyTemplate()
        {
            return new TimerClientData()
            {
                Name = "Timer",
                Hours = 10,
                Minutes = 30,
                UserBrightness = 0.5f,
                Shading = 0f,
            };
        }

        public async Task<CommandResponse<Timer>> ExecuteAsync(CloudClient client, long installationID, TimerClientData clientData)
        {
            var request = CreateRequest(client);
            request.AddUrlSegment("InstallationID", installationID.ToString());
            request.AddJsonBody(clientData);

            return await this.ExecuteRequestExactAsync(client, request);
        }

    }

}
