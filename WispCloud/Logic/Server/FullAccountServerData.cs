using System.Collections.Generic;
using DeusCloud.Data.Entities.Accounts;
using DeusCloud.Data.Entities.GameEvents;

namespace DeusCloud.Logic.Server
{
    public class FullAccountServerData
    {
        public Account User { get; set; }
        public List<GameEvent> History { get; set; }
        
        public FullAccountServerData(Account user)
        {
            User = user;
        }
    }
}