﻿using RestSharp;
using System.Threading.Tasks;
using WispCloudClient.ApiTypes;

namespace WispCloudClient.ApiCommands
{
    public sealed class DeleteAccountFromInstallationCommand : InputCommand<string>
    {
        public override Method Method { get { return Method.DELETE; } }
        public override string Name { get { return "Delete user from installation"; } }
        public override string Resource { get { return "api/installations/{InstallationID}/accounts"; } }
        public override AccountRoles Roles
        {
            get { return AccountRoles.SeviceEnginier | AccountRoles.User | AccountRoles.Hub; }
        }

        protected override object GetRequestBodyTemplate()
        {
            return "email@example.com";
        }

        public async Task<CommandResponse> ExecuteAsync(CloudClient client, long installationID, string email)
        {
            var request = CreateRequest(client);
            request.AddUrlSegment("InstallationID", installationID.ToString());
            request.AddJsonBody(email);

            return await ExecuteRequestAsync(client, request);
        }

    }

}
