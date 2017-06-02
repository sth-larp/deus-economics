using RestSharp;
using System.Threading.Tasks;
using WispCloudClient.ApiTypes;

namespace WispCloudClient.ApiCommands
{
    public sealed class EditGroupCommand : InputOutputCommand<GroupClientData, GroupItem>
    {
        public override Method Method { get { return Method.PUT; } }
        public override string Name { get { return "Edit group"; } }
        public override string Resource
        {
            get { return "api/installations/{InstallationID}/groups/{GroupID}"; }
        }
        public override AccountRoles Roles { get { return AccountRoles.SeviceEnginier | AccountRoles.User; } }

        protected override object GetRequestBodyTemplate()
        {
            return new GroupClientData()
            {
                Name = "Edited group",
            };
        }

        public async Task<CommandResponse<GroupItem>> ExecuteAsync(CloudClient client, long installationID, int groupId, GroupClientData clientData)
        {
            var request = CreateRequest(client);
            request.AddUrlSegment("InstallationID", installationID.ToString());
            request.AddUrlSegment("GroupID", groupId.ToString());
            request.AddJsonBody(clientData);

            return await this.ExecuteRequestExactAsync(client, request);
        }

    }

}
