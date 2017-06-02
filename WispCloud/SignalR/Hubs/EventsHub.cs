using Microsoft.AspNet.SignalR;

namespace WispCloud.SignalR.Hubs
{
    public sealed class EventsHub : BaseHub
    {
        public static IHubContext HubContext
        {
            get { return GlobalHost.ConnectionManager.GetHubContext<EventsHub>(); }
        }

    }

}