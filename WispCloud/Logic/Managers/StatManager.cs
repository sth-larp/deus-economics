using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using DeusCloud.Data.Entities.Accounts;
using DeusCloud.Data.Entities.Alice;
using DeusCloud.Data.Entities.GameEvents;
using DeusCloud.Data.Entities.Transactions;
using DeusCloud.Helpers;
using DeusCloud.Logic.Client;
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

        public TranStatServerData GetTransactionStat()
        {
            UserContext.Rights.CheckRole(AccountRole.Admin);

            var data = new TranStatServerData();
            var from = UserContext.Constants.LastCycleDate();
            var allTrans = UserContext.Data.Transactions
                .Where(x => x.Type != TransactionType.Payment && x.Time > from).ToList();

            var receiverDict = new Dictionary<string, TranStatElement>();
            var senderDict = new Dictionary<string, TranStatElement>();

            allTrans.Where(x => x.Type!= TransactionType.Tax).ToList().ForEach(x =>
            {
                if (!receiverDict.ContainsKey(x.Receiver))
                    receiverDict.Add(x.Receiver, new TranStatElement(x.Receiver, 0)
                    {
                        Fullname = x.ReceiverAccount.DisplayName,
                        Role = x.ReceiverAccount.Role
                    });

                if (!senderDict.ContainsKey(x.Sender))
                    senderDict.Add(x.Sender, new TranStatElement(x.Sender, 0)
                    {
                        Fullname = x.SenderAccount.DisplayName,
                        Role = x.SenderAccount.Role
                    });

                receiverDict[x.Receiver].Amount += x.Amount;
                senderDict[x.Sender].Amount += x.Amount;
            });

            data.Top20CompReceivers = receiverDict
                .Select(x => x.Value).Where(x => x.Role >= AccountRole.Corp)
                .OrderByDescending(x => x.Amount)
                .Take(20).ToList();

            data.Top20CompSenders = senderDict
                .Select(x => x.Value).Where(x => x.Role >= AccountRole.Corp)
                .OrderByDescending(x => x.Amount)
                .Take(20).ToList();

            data.Top50PersReceivers = receiverDict
                .Select(x => x.Value).Where(x => x.Role == AccountRole.Person)
                .OrderByDescending(x => x.Amount)
                .Take(50).ToList();

            var allUsers = UserContext.Data.Accounts.Where(x => x.Role != AccountRole.Master
                                                                && x.Role != AccountRole.Admin).ToList();
            data.Cash = allUsers.Select(x => x.Cash).Aggregate((sum, x) => x + sum);

            data.CashOut = allTrans.Where(x => x.ReceiverAccount.Role == AccountRole.Master 
                || x.ReceiverAccount.Role == AccountRole.Admin).Select(x => x.Amount)
                .Aggregate((sum, x) => sum + x);

            return data;
        }

        public StatServerData GetAliceStat(bool ingame)
        {
            UserContext.Rights.CheckRole(AccountRole.Admin);

            var data = new StatServerData();
            var models = GetAliceData();

            var characters = models.Where(x => x.isAlive && (ingame? x.inGame : true)).ToList();
            var implants = characters.SelectMany(x => x.modifiers).ToList();
            //var robots = characters.Select(x => x.profileType == "robot").ToList();

            var pa = 0;
            var jj = 0;
            var s = 0;

            implants.Where(x => x.id != null).ToList().ForEach(x =>
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

            UserContext.Constants.EditConstant(new ConstantClientData() { Name = "CurrentVR", Value = data.TimeInVR });
            return data;
        }

        public void BanAndroids()
        {
            UserContext.Rights.CheckRole(AccountRole.Admin);
            var data = new BlockedServerData();

            var models = GetAliceData();

            models.ForEach(x =>
            {
                if (!x.isAlive)
                    data.Deads += BlockCharWithReason(x._id, "смерть") ? 1 : 0;
                else if (!x.inGame)
                    data.NotInGame += BlockCharWithReason(x._id, "не в игре") ? 1 : 0;
                else if (x.profileType != "human")
                    data.Robots += BlockCharWithReason(x._id, "вы не человек") ? 1 : 0;
            });
        }

        private bool BlockCharWithReason(string login, string reason)
        {
            var acc = UserContext.Accounts.Get(login);
            if (acc == null) return false;
            if (acc.Status != AccountStatus.Active) return false;

            acc.Status = AccountStatus.Blocked;
            UserContext.Data.SaveChanges();
            UserContext.AddGameEvent(acc.Login, GameEventType.None, $"Аккаунт заблокирован, причина: {reason}");
            return true;
        }

        public List<AliceModel> GetAliceData()
        {
            var url = AppSettings.Url("AliceUrl") + "/models/_all_docs";

            var webClient = new WebClient();
            webClient.QueryString.Add("include_docs", "true");
            webClient.Headers.Add("Authorization", "Basic ZWNvbm9taWNzOno2dHJFKnJVRHJ1OA==");
            webClient.Headers.Add("Accept", "application/json");
            webClient.Headers.Add("Content-type", "application/json");
            string result = webClient.DownloadString(url);

            var models = JsonConvert.DeserializeObject<AliceModels>(result);
            var characters = models.rows.Select(x => x.doc).ToList();
            return characters;
        }

    }
}