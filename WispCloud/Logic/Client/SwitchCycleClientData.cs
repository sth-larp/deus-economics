using System.Collections.Generic;
using DeusCloud.Logic.CommonBase;
using Newtonsoft.Json;

namespace DeusCloud.Logic.Client
{
    public sealed class SwitchCycleClientData : BaseModel
    {
        public int IndexPanam { get; set; }
        public int IndexSerenity { get; set; }
        public int IndexJJ { get; set; }
        public int IndexGovt { get; set; }

        [JsonIgnore]
        public KeyValuePair<string, int>[] Indexes
        {
            get
            {
                var ret = new KeyValuePair<string, int>[4];
                ret[0] = new KeyValuePair<string, int>("Panam", IndexPanam);
                ret[1] = new KeyValuePair<string, int>("JJ", IndexJJ);
                ret[2] = new KeyValuePair<string, int>("Serenity", IndexSerenity);
                ret[3] = new KeyValuePair<string, int>("Govt", IndexGovt);
                return ret;
            }
        }

        public SwitchCycleClientData()
        {
        }

        public override void Validate()
        {
        }
    }
}