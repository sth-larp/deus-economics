using RestSharp;
using System.Threading.Tasks;
using WispCloudClient.ApiTypes;

namespace WispCloudClient.ApiCommands
{
    public sealed class AddButtonWallSwitchToInstallationCommand : InputCommand<string>
    {
        public override Method Method { get { return Method.POST; } }
        public override string Name { get { return "Add button wall switch to installation"; } }
        public override string Resource { get { return "api/installations/{InstallationID}/bws"; } }
        public override AccountRoles Roles { get { return AccountRoles.SeviceEnginier | AccountRoles.User; } }

        protected override object GetRequestBodyTemplate()
        {
            return "0";
        }

        public async Task<CommandResponse> ExecuteAsync(CloudClient client, long installationID, string buttonWallSwitchSN)
        {
            var request = CreateRequest(client);
            request.AddUrlSegment("InstallationID", installationID.ToString());
            request.AddJsonBody(buttonWallSwitchSN);

            return await ExecuteRequestAsync(client, request);
        }

    }

}
