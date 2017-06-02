using RestSharp;
using System.Threading.Tasks;
using WispCloudClient.ApiTypes;

namespace WispCloudClient.ApiCommands
{
    public sealed class GetCurrentAccountCommand : OutputCommand<Account>
    {
        public override Method Method { get { return Method.GET; } }
        public override string Name { get { return "Get current account"; } }
        public override string Resource { get { return "api/accounts/current"; } }
        public override AccountRoles Roles
        {
            get { return AccountRoles.SeviceEnginier | AccountRoles.User | AccountRoles.Hub; }
        }

        public async Task<CommandResponse<Account>> ExecuteAsync(CloudClient client)
        {
            var request = CreateRequest(client);

            return await this.ExecuteRequestExactAsync(client, request);
        }

    }

}
