using RestSharp;
using System.Collections.Generic;
using System.Threading.Tasks;
using WispCloudClient.ApiTypes;

namespace WispCloudClient.ApiCommands
{
    public sealed class UpdateButtonWallSwitchesCommand : InputCommand<List<ButtonWallSwitchStatusClientData>>
    {
        public override Method Method { get { return Method.PUT; } }
        public override string Name { get { return "Update button wall switches"; } }
        public override string Resource { get { return "api/installations/{InstallationID}/bws/status"; } }
        public override AccountRoles Roles { get { return AccountRoles.Hub; } }

        protected override object GetRequestBodyTemplate()
        {
            return new[] {
                new ButtonWallSwitchStatusClientData() { ButtonWallSwitchSN = "1", Battery = 0.5f },
                new ButtonWallSwitchStatusClientData() { ButtonWallSwitchSN = "2", Battery = 0.5f },
            };
        }

        protected override object GenerateBodyRequest()
        {
            var BWSs = new List<ButtonWallSwitchStatusClientData>();
            for (int i = 0; i < StaticRandom.Next(2) + 2; i++)
                BWSs.Add(new ButtonWallSwitchStatusClientData()
                { ButtonWallSwitchSN = (i + 1).ToString(), Battery = StaticRandom.Next(100) / 100f });

            return BWSs;
        }

        public async Task<CommandResponse> ExecuteAsync(CloudClient client, long installationID, List<ButtonWallSwitchStatusClientData> clientData)
        {
            var request = CreateRequest(client);
            request.AddUrlSegment("InstallationID", installationID.ToString());
            request.AddJsonBody(clientData);

            return await ExecuteRequestAsync(client, request);
        }

    }

}
