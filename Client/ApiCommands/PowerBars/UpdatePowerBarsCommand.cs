using RestSharp;
using System.Collections.Generic;
using System.Threading.Tasks;
using WispCloudClient.ApiTypes;

namespace WispCloudClient.ApiCommands
{
    public sealed class UpdatePowerBarsCommand : InputCommand<List<PowerBarStatusClientData>>
    {
        public override Method Method { get { return Method.PUT; } }
        public override string Name { get { return "Update power bars"; } }
        public override string Resource { get { return "api/installations/{InstallationID}/powerbars/status"; } }
        public override AccountRoles Roles { get { return AccountRoles.Hub; } }

        protected override object GetRequestBodyTemplate()
        {
            return new[]
            {
                new PowerBarStatusClientData() { PowerBarSN = "1", Battery = 0.5f, RealBrightness = 0.5f, Radio = -3 },
                new PowerBarStatusClientData() { PowerBarSN = "2", Battery = 0.5f, RealBrightness = 0.5f, Radio = 1 },
            };
        }

        protected override object GenerateBodyRequest()
        {
            var BWSs = new List<PowerBarStatusClientData>();
            for (int i = 0; i < StaticRandom.Next(20) + 2; i++)
            {
                BWSs.Add(new PowerBarStatusClientData()
                {
                    PowerBarSN = (i + 1).ToString(),
                    Battery = StaticRandom.Next(100) / 100f,
                    RealBrightness = StaticRandom.Next(100) / 100f,
                    Radio = StaticRandom.Next(20) - 15
                });
            }

            return BWSs;
        }

        public async Task<CommandResponse> ExecuteAsync(CloudClient client, long installationID, List<PowerBarStatusClientData> clientData)
        {
            var request = CreateRequest(client);
            request.AddUrlSegment("InstallationID", installationID.ToString());
            request.AddJsonBody(clientData);

            return await ExecuteRequestAsync(client, request);
        }

    }

}
