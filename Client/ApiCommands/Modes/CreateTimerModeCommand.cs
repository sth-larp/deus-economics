using RestSharp;
using System.Threading.Tasks;
using WispCloudClient.ApiTypes;

namespace WispCloudClient.ApiCommands
{
    public sealed class CreateTimerModeCommand : InputOutputCommand<TimerModeClientData, TimerMode>
    {
        public override Method Method { get { return Method.POST; } }
        public override string Name { get { return "Create timer mode"; } }
        public override string Resource { get { return "api/installations/{InstallationID}/modes"; } }
        public override AccountRoles Roles { get { return AccountRoles.SeviceEnginier | AccountRoles.User; } }

        protected override object GetRequestBodyTemplate()
        {
            return new TimerModeClientData()
            {
                Name = "Timer",
                Settings = new TimerModeSettings()
                {
                    Hours = 10,
                    Minutes = 30,
                    UserBrightness = 50,
                    Shading = 0,
                }
            };
        }

        public async Task<CommandResponse<TimerMode>> ExecuteAsync(CloudClient client, long installationID, TimerModeClientData clientData)
        {
            var request = CreateRequest(client);
            request.AddUrlSegment("InstallationID", installationID.ToString());
            request.AddJsonBody(clientData);

            return await this.ExecuteRequestExactAsync(client, request);
        }

    }

}
