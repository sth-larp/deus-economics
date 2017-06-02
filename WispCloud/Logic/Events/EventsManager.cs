using System;
using System.Collections.Generic;
using System.Linq;
using DeusCloud.Logic.CommonBase;
using Microsoft.AspNet.SignalR;
using WispCloud.Logic;
using WispCloud.Logic.EventArgs;
using WispCloud.SignalR.Hubs;

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

        List<string> GetLoginsForInstallation(long installationID)
        {
            return UserContext.Data.AccountAccesses
                .Where(x => x.InstallationID == installationID)
                .Select(x => x.Login)
                .ToList();
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