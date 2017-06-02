using Microsoft.AspNet.SignalR;
using System.Threading.Tasks;
using DeusCloud.Logic;
using WispCloud.Logic;

namespace WispCloud.SignalR.Hubs
{
    public abstract class BaseHub : Hub
    {
        public override Task OnConnected()
        {
            UserContext.SignalRConnectionMapping.UpdateConnection(Context);
            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            UserContext.SignalRConnectionMapping.RemoveConnection(Context);
            return base.OnDisconnected(stopCalled);
        }

        public override Task OnReconnected()
        {
            UserContext.SignalRConnectionMapping.UpdateConnection(Context);
            return base.OnReconnected();
        }

    }
}