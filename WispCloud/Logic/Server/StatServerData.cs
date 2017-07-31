using System.Collections.Generic;
using DeusCloud.Data.Entities.Accounts;

namespace DeusCloud.Logic.Server
{
    public class StatServerData
    {
        public Dictionary<InsuranceType, int> Implants { get; set; }
        public int TimeInVR { get; set; }

        public StatServerData()
        {
            Implants = new Dictionary<InsuranceType, int>();
        }
    }
}