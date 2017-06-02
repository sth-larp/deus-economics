using RestSharp;
using System.Threading.Tasks;
using WispCloudClient.ApiTypes;

namespace WispCloudClient.ApiCommands
{
    public sealed class EditButtonWallSwitchCommand : InputOutputCommand<ButtonWallSwitchUserClientData, ButtonWallSwitch>
    {
        public override Method Method { get { return Method.PUT; } }
        public override string Name { get { return "Edit button wall switch"; } }
        public override string Resource
        {
            get { return "api/installations/{InstallationID}/bws/{ButtonWallSwitchSN}"; }
        }
        public override AccountRoles Roles { get { return AccountRoles.SeviceEnginier | AccountRoles.User; } }

        protected override object GetRequestBodyTemplate()
        {
            return new ButtonWallSwitchUserClientData()
            {
                Name = "Edited button wall switch",
                FirstPreset = new ButtonWallSwitchSettings(),
                SecondPreset = new ButtonWallSwitchSettings(),
                Containers = new System.Collections.Generic.List<ContainerToButtonWallSwitchClientData>()
                {
                    new ContainerToButtonWallSwitchClientData() { ContainerID = 1, ShortName = "ALL" },
                },
            };
        }

        public async Task<CommandResponse<ButtonWallSwitch>> ExecuteAsync(CloudClient client, long installationID, string buttonWallSwitchSN, ButtonWallSwitchUserClientData clientData)
        {
            var request = CreateRequest(client);
            request.AddUrlSegment("InstallationID", installationID.ToString());
            request.AddUrlSegment("ButtonWallSwitchSN", buttonWallSwitchSN);
            request.AddJsonBody(clientData);

            return await this.ExecuteRequestExactAsync(client, request);
        }

    }

}
