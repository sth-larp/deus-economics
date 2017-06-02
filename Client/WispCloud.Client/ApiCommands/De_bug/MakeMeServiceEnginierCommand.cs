using RestSharp;
using System.Threading.Tasks;
using WispCloudClient.ApiTypes;

namespace WispCloudClient.ApiCommands
{
    public sealed class MakeMeServiceEnginierCommand : BaseCommand
    {
        public override Method Method { get { return Method.GET; } }
        public override string Name { get { return "DEBUG: make current user service enginier"; } }
        public override string Resource { get { return "api/debug/makemeserviceenginier"; } }
        public override AccountRoles Roles
        {
            get { return AccountRoles.SeviceEnginier | AccountRoles.User | AccountRoles.Hub; }
        }
        public override int SortIndex { get { return 10; } }

        public async Task<CommandResponse> ExecuteAsync(CloudClient client)
        {
            var request = CreateRequest(client);

            return await this.ExecuteRequestAsync(client, request);
        }

    }

}
