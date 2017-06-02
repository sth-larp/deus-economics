using RestSharp;
using WispCloudClient.ApiTypes;

namespace WispCloudClient.ApiCommands
{
    public sealed class EmptyCommand : BaseCommand
    {
        public override Method Method { get { return Method.GET; } }
        public override string Name { get { return "<empty>"; } }
        public override string Resource { get { return "api/"; } }
        public override AccountRoles Roles
        {
            get { return AccountRoles.SeviceEnginier | AccountRoles.User | AccountRoles.Hub; }
        }
        public override int SortIndex { get { return 0; } }

    }

}
