using RestSharp;
using System.Collections.Generic;
using System.Threading.Tasks;
using WispCloudClient.ApiTypes;

namespace WispCloudClient.ApiCommands
{
    public sealed class PowerBarsActionCommand : InputOutputCommand<PowerBarActionClientData, List<PowerBar>>
    {
        public override Method Method { get { return Method.POST; } }
        public override string Name { get { return "Power bars action"; } }
        public override string Resource { get { return "api/installations/{InstallationID}/powerbars/action"; } }
        public override AccountRoles Roles
        {
            get { return AccountRoles.SeviceEnginier | AccountRoles.User | AccountRoles.Hub; }
        }

        protected override object GetRequestBodyTemplate()
        {
            return new PowerBarActionClientData()
            {
                PowerBarsParams = new[]
                {
                    new PowerBarActionParamsClientData()
                }
            };
        }

        public async Task<CommandResponse<List<PowerBar>>> ExecuteAsync(CloudClient client, long installationID, PowerBarActionClientData clientData)
        {
            var request = CreateRequest(client);
            request.AddUrlSegment("InstallationID", installationID.ToString());
            request.AddJsonBody(clientData);

            return await this.ExecuteRequestExactAsync(client, request);
        }

    }

}
