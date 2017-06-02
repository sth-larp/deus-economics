using RestSharp;
using System.Threading.Tasks;
using WispCloudClient.ApiTypes;

namespace WispCloudClient.ApiCommands
{
    public sealed class EditPowerBarCommand : InputOutputCommand<PowerBarUserClientData, PowerBar>
    {
        public override Method Method { get { return Method.PUT; } }
        public override string Name { get { return "Edit power bar"; } }
        public override string Resource
        {
            get { return "api/installations/{InstallationID}/powerbars/{PowerBarSN}"; }
        }
        public override AccountRoles Roles { get { return AccountRoles.SeviceEnginier | AccountRoles.User; } }

        protected override object GetRequestBodyTemplate()
        {
            return new PowerBarUserClientData()
            {
                Name = "Edited power bar",
            };
        }

        public async Task<CommandResponse<PowerBar>> ExecuteAsync(CloudClient client, long installationID, string powerBarSN, PowerBarUserClientData clientData)
        {
            var request = CreateRequest(client);
            request.AddUrlSegment("InstallationID", installationID.ToString());
            request.AddUrlSegment("PowerBarSN", powerBarSN);
            request.AddJsonBody(clientData);

            return await this.ExecuteRequestExactAsync(client, request);
        }

    }

}
