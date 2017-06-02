using RestSharp;
using System.Threading.Tasks;
using WispCloudClient.ApiTypes;

namespace WispCloudClient.ApiCommands
{
    public sealed class CreateInstallationCommand : OutputCommand<InstallationClientData>
    {
        public override Method Method { get { return Method.POST; } }
        public override string Name { get { return "Create installation"; } }
        public override string Resource { get { return "api/installations"; } }
        public override AccountRoles Roles { get { return AccountRoles.SeviceEnginier | AccountRoles.User; } }

        public async Task<CommandResponse<InstallationClientData>> ExecuteAsync(CloudClient client)
        {
            var request = CreateRequest(client);

            return await this.ExecuteRequestExactAsync(client, request);
        }

    }

}
