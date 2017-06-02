using RestSharp;
using System.Threading.Tasks;
using WispCloudClient.ApiTypes;

namespace WispCloudClient.ApiCommands
{
    public sealed class AddPowerBarToInstallationCommand : InputCommand<PowerBarAddToInstallationClientData>
    {
        public override Method Method { get { return Method.POST; } }
        public override string Name { get { return "Add power bar to installation"; } }
        public override string Resource { get { return "api/installations/{InstallationID}/powerbars"; } }
        public override AccountRoles Roles { get { return AccountRoles.SeviceEnginier | AccountRoles.User; } }

        protected override object GetRequestBodyTemplate()
        {
            return new PowerBarAddToInstallationClientData()
            {
                PowerBarSN = "0",
                WindowID = 0,
            };
        }

        public async Task<CommandResponse> ExecuteAsync(CloudClient client, long installationID, PowerBarAddToInstallationClientData clietnData)
        {
            var request = CreateRequest(client);
            request.AddUrlSegment("InstallationID", installationID.ToString());
            request.AddJsonBody(clietnData);

            return await ExecuteRequestAsync(client, request);
        }

    }

}
