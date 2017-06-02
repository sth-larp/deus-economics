using RestSharp;
using System.Threading.Tasks;
using WispCloudClient.ApiTypes;

namespace WispCloudClient.ApiCommands
{
    public sealed class NewGroupCommand : InputOutputCommand<GroupClientData, GroupItem>
    {
        public override Method Method { get { return Method.POST; } }
        public override string Name { get { return "Create group"; } }
        public override string Resource { get { return "api/installations/{InstallationID}/groups"; } }
        public override AccountRoles Roles { get { return AccountRoles.SeviceEnginier | AccountRoles.User; } }

        protected override object GetRequestBodyTemplate()
        {
            return new GroupClientData()
            {
                Name = "New group",
            };
        }

        public async Task<CommandResponse<GroupItem>> ExecuteAsync(CloudClient client, long installationID, GroupClientData clientData)
        {
            var request = CreateRequest(client);
            request.AddUrlSegment("InstallationID", installationID.ToString());
            request.AddJsonBody(clientData);

            return await this.ExecuteRequestExactAsync(client, request);
        }

    }

}
