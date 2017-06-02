using System.Collections.Generic;
using System.Linq;
using DeusCloud.Logic.CommonBase;
using DeusCloud.Logic.Events.Client;
using DeusCloud.SignalR.Hubs;
using Microsoft.AspNet.SignalR;

namespace DeusCloud.Logic.Events
{
    public class EventsManager : ContextHolder
    {
        IHubContext _hubContext;

        public EventsManager(UserContext context)
            : base(context)
        {
            this._hubContext = EventsHub.HubContext;
        }

        public void InstallationChange(long installationID, EventActionType action)
        {
            var connectionIDs = UserContext.SignalRConnectionMapping
                .GetConnectionIDsForLoginExceptAuthorization(UserContext.CurrentUser.Login, UserContext.CurrentAuthorization);

            _hubContext.Clients.Clients(connectionIDs).InstallationChange(
                new InstallationEventArgs()
                {
                    Action = action,
                    InstallationID = installationID,
                });
        }

        
    }

}