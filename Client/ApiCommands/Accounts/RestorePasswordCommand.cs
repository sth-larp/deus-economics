﻿using System.Threading.Tasks;
using DeusClient.ApiCommands.Accounts.Client;
using DeusClient.ApiCommands.CommonBase;
using DeusClient.Client;
using RestSharp;

namespace DeusClient.ApiCommands.Accounts
{
    public sealed class RestorePasswordCommand : InputCommand<string>
    {
        public override Method Method { get { return Method.POST; } }
        public override string Name { get { return "Restore password"; } }
        public override string Resource { get { return "api/accounts/restorepassword"; } }
        public override AccountRoles Roles
        {
            get { return AccountRoles.SeviceEnginier | AccountRoles.User | AccountRoles.Hub; }
        }

        protected override object GetRequestBodyTemplate()
        {
            return "email@example.com";
        }

        public async Task<CommandResponse> ExecuteAsync(CloudClient client, string email)
        {
            var request = CreateRequest(client);
            request.AddJsonBody(email);

            return await ExecuteRequestAsync(client, request);
        }

    }

}
