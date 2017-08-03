using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DeusCloud.Data.Entities.Alice
{
    public class AliceModels
    {
        public List<AliceDoc> rows { get; set; }
    }

    public class AliceDoc
    {
        public AliceModel doc { get; set; }
    }

    public class AliceModel
    {
        public string _id { get; set; }
        public string profileType { get; set; }
        public bool isAlive { get; set; }
        public bool inGame { get; set; }
        public int totalSpentInVR { get; set; }
        public List<AliceModifier> modifiers { get; set; }
    }

    public class AliceModifier
    {
        public string @class { get; set; }
        public string id { get; set; }
        public bool enabled { get; set; }
    }
}