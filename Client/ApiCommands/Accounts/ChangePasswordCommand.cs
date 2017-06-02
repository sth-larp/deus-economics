using RestSharp;
using System.Threading.Tasks;
using WispCloudClient.ApiTypes;

namespace WispCloudClient.ApiCommands
{
    public class ChangePasswordCommand : InputCommand<ChangePasswordClientData>
    {
        public override Method Method { get { return Method.POST; } }
        public override string Name { get { return "Change account password"; } }
        public override string Resource { get { return "api/accounts/changepassword"; } }
        public override AccountRoles Roles
        {
            get { return AccountRoles.SeviceEnginier | AccountRoles.User | AccountRoles.Hub; }
        }

        protected override object GetRequestBodyTemplate()
        {
            return new ChangePasswordClientData()
            {
                Login = "email@example.com",
                CurrentPassword = "********",
                NewPassword = "********",
            };
        }

        public async Task<CommandResponse> ExecuteAsync(CloudClient client, ChangePasswordClientData clientData)
        {
            var request = CreateRequest(client);
            request.AddJsonBody(clientData);

            return await ExecuteRequestAsync(client, request);
        }

    }

}
