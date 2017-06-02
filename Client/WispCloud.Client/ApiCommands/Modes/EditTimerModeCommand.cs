using RestSharp;
using System.Threading.Tasks;
using WispCloudClient.ApiTypes;

namespace WispCloudClient.ApiCommands
{
    class EditTimerModeCommand : InputOutputCommand<TimerModeClientData, TimerMode>
    {
        public override Method Method { get { return Method.PUT; } }
        public override string Name { get { return "Edit timer mode"; } }
        public override string Resource { get { return "api/installations/{InstallationID}/modes/{ModeID}"; } }
        public override AccountRoles Roles { get { return AccountRoles.SeviceEnginier | AccountRoles.User; } }

        protected override object GetRequestBodyTemplate()
        {
            return new TimerModeClientData()
            {
                Name = "Edited timer",
                Settings = new TimerModeSettings()
                {
                    Hours = 18,
                    Minutes = 10,
                    UserBrightness = 50,
                    Shading = 0,
                }
            };
        }

        public async Task<CommandResponse<TimerMode>> ExecuteAsync(CloudClient client, long installationID, int modeID, TimerModeClientData clientData)
        {
            var request = CreateRequest(client);
            request.AddUrlSegment("InstallationID", installationID.ToString());
            request.AddUrlSegment("ModeID", modeID.ToString());
            request.AddJsonBody(clientData);

            return await this.ExecuteRequestExactAsync(client, request);
        }

    }

}
