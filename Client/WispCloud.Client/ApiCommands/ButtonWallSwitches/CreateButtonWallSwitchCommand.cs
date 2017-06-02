using RestSharp;
using System.Threading.Tasks;
using WispCloudClient.ApiTypes;

namespace WispCloudClient.ApiCommands
{
    public sealed class CreateButtonWallSwitchCommand : InputCommand<string>
    {
        public override Method Method { get { return Method.POST; } }
        public override string Name { get { return "Create button wall switch"; } }
        public override string Resource { get { return "api/bws"; } }
        public override AccountRoles Roles { get { return AccountRoles.SeviceEnginier | AccountRoles.User; } }

        protected override object GetRequestBodyTemplate()
        {
            return "1";
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
