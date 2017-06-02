using System.Threading.Tasks;
using DeusClient.ApiCommands.Accounts.Client;
using DeusClient.ApiCommands.CommonBase;
using DeusClient.Client;
using RestSharp;

namespace DeusClient.ApiCommands.Accounts
{
    public sealed class LoginCommand : OutputCommand<Authorization>
    {
        public override Method Method { get { return Method.POST; } }
        public override string Name { get { return "Login"; } }
        public override string Resource { get { return "oauth2/token"; } }
        public override AccountRoles Roles
        {
            get { return AccountRoles.SeviceEnginier | AccountRoles.User | AccountRoles.Hub; }
        }

        public async Task<CommandResponse<Authorization>> ExecuteAsync(CloudClient client, string login, string password)
        {
            var request = CreateRequest(client);
            request.AddParameter("grant_type", "password");
            request.AddParameter("username", login);
            request.AddParameter("password", password);

            return await this.ExecuteRequestExactAsync(client, request);
        }

    }
}
