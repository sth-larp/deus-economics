﻿using System.Threading.Tasks;
using DeusClient.ApiCommands.Accounts.Client;
using DeusClient.ApiCommands.CommonBase;
using DeusClient.Client;
using RestSharp;

namespace DeusClient.ApiCommands.Accounts
{
    public sealed class RegistrationCommand : InputOutputCommand<RegistrationClientData, Account>
    {
        public override Method Method { get { return Method.POST; } }
        public override string Name { get { return "Registers new user"; } }
        public override string Resource { get { return "api/accounts"; } }
        public override AccountRoles Roles
        {
            get { return AccountRoles.SeviceEnginier | AccountRoles.User | AccountRoles.Hub; }
        }

        public async Task<CommandResponse<Account>> ExecuteAsync(CloudClient client, RegistrationClientData clientData)
        {
            var request = CreateRequest(client);
            request.AddJsonBody(clientData);

            return await this.ExecuteRequestExactAsync(client, request);
        }

    }

}
