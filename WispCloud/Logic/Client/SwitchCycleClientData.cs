using System.Collections.Generic;
using DeusCloud.Logic.CommonBase;

namespace DeusCloud.Logic.Client
{
    public sealed class SwitchCycleClientData : BaseModel
    {
        public KeyValuePair<string, int>[] Indexes { get; set; }

        public SwitchCycleClientData()
        {
        }

        public override void Validate()
        {
        }
    }
}