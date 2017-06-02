using System.Collections.Generic;
using System.Threading.Tasks;
using DeusClient.ApiCommands.Accounts.Client;
using DeusClient.ApiCommands.CommonBase;
using DeusClient.Client;
using RestSharp;

namespace DeusClient.ApiCommands.Accounts
{
    public class EditAccountRolesCommand : InputCommand<RolesClientData>
    {
        public override Method Method { get { return Method.POST; } }
        public override string Name { get { return "Edit account roles"; } }
        public override string Resource { get { return "api/accounts/roles"; } }
        public override AccountRoles Roles
        {
            get { return AccountRoles.SeviceEnginier | AccountRoles.User | AccountRoles.Hub; }
        }

        protected override object GetRequestBodyTemplate()
        {
            return new RolesClientData()
            {
                Login = "email@example.com",
                Roles = new List<AccountRoles>() { AccountRoles.User, AccountRoles.SeviceEnginier },
                Active = true,
            };
        }

        public async Task<CommandResponse> ExecuteAsync(CloudClient client, RolesClientData clientData)
        {
            var request = CreateRequest(client);
            request.AddJsonBody(clientData);

            return await this.ExecuteRequestAsync(client, request);
        }

    }

}
