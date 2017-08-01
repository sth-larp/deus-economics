using System.Collections.Generic;
using DeusCloud.Data.Entities.Accounts;
using Newtonsoft.Json;

namespace DeusCloud.Logic.Server
{
    public class TranStatServerData
    {
        public List<TranStatElement> Top20CompReceivers { get; set; }

        public List<TranStatElement> Top20CompSenders { get; set; }

        public List<TranStatElement> Top50PersReceivers { get; set; }

        public float Cash { get; set; }

        public float CashOut { get; set; }

        public TranStatServerData()
        {
            Top20CompReceivers = new List<TranStatElement>();
            Top20CompSenders = new List<TranStatElement>();
            Top50PersReceivers = new List<TranStatElement>();
        }
    }

    public class TranStatElement
    {
        public string Login { get; set; }
        public string Fullname { get; set; }
        public float Amount { get; set; }

        [JsonIgnore]
        public AccountRole Role { get; set; }

        public TranStatElement(string login, float amount)
        {
            Login = login;
            Amount = amount;
        }
        
    }
}