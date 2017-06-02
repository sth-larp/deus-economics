using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using WispCloud.Data;

namespace WispCloud.Logic
{
    public sealed class PowerBarItem
    {
        [JsonIgnore]
        public PowerBar PowerBar { get; }

        [JsonIgnore]
        public int? SecurityKeyID { get { return PowerBar.SecurityKeyID; } }

        [JsonIgnore]
        public decimal? EPID { get { return PowerBar.EPID; } }

        public decimal PowerBarSN { get { return PowerBar.PowerBarSN; } }
        public string Name { get { return PowerBar.Name; } }
        public string ShortName { get { return PowerBar.ShortName; } }
        public int? ModeID { get { return PowerBar.ModeID; } }
        public int Battery { get { return PowerBar.Battery; } }
        public int Shading { get { return PowerBar.Shading; } }
        public int RealBrightness { get { return PowerBar.RealBrightness; } }
        public int UserBrightness { get { return PowerBar.UserBrightness; } }
        public NetworkStatus Status { get { return PowerBar.Status; } }

        [JsonIgnore]
        public List<GroupItem> Groups { get; set; }

        public IEnumerable<int> GroupIDs
        {
            get
            {
                if (Groups == null)
                    return null;

                return Groups.Select(x => x.GroupID);
            }
        }

        public PowerBarItem(PowerBar powerBar)
        {
            this.PowerBar = powerBar;
        }

    }

}