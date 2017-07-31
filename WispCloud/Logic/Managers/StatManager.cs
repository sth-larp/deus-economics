using System.Collections.Generic;
using System.Linq;
using System.Net;
using DeusCloud.Data.Entities.Accounts;
using DeusCloud.Helpers;
using DeusCloud.Logic.CommonBase;
using DeusCloud.Logic.Server;
using Newtonsoft.Json;

namespace DeusCloud.Logic.Managers
{
    public class StatManager : ContextHolder
    {
        public StatManager(UserContext context) : base(context)
        {
        }

        public StatServerData GetStatistics(bool ingame)
        {
            var data = new StatServerData();

            var json = GetAliceData("/accounts/_all_docs");
            var parsed = JsonConvert.DeserializeObject<AliceModels>(json);

            var characters = parsed.rows.Select(x => x.doc).Where(x => x.isAlive && (ingame? x.inGame : true)).ToList();
            var implants = characters.SelectMany(x => x.modifiers).ToList(); 

            var pa = 0;
            var jj = 0;
            var s = 0;

            implants.ForEach(x =>
            {
                if (x.id.StartsWith("jj_")) jj++;
                else if (x.id.StartsWith("pa_")) pa++;
                else if (x.id.StartsWith("s_")) s++;
            });

            data.Implants.Add(InsuranceType.Panam, pa);
            data.Implants.Add(InsuranceType.Serenity, s);
            data.Implants.Add(InsuranceType.JJ, jj);

            characters.ForEach(x => data.TimeInVR += x.totalSpentInVR);
            data.TimeInVR /= 1000;
            return data;
        }

        public string GetAliceData(string path)
        {
            var url = AppSettings.Url("AliceUrl") + "/models/_all_docs";

            var webClient = new WebClient();
            webClient.QueryString.Add("include_docs", "true");
            webClient.Headers.Add("Authorization", "Basic ZWNvbm9taWNzOno2dHJFKnJVRHJ1OA==");
            webClient.Headers.Add("Accept", "application/json");
            webClient.Headers.Add("Content-type", "application/json");
            string result = webClient.DownloadString(url);

            return result;
        }

        private class AliceModels
        {
            public List<AliceDoc> rows { get; set; }
        }

        private class AliceDoc
        {
            public AliceModel doc { get; set; }
        }

        private class AliceModel 
        {
            public string _id { get; set; }
            public bool isAlive { get; set; }
            public bool inGame { get; set; }
            public int totalSpentInVR { get; set; }
            public List<AliceModifier> modifiers { get; set; }
        }

        private class AliceModifier
        {
            public string @class { get; set; }
            public string id { get; set; }
            public bool enabled { get; set; }
        }
    }
}