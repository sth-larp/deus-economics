using System;
using DeusCloud.Data.Entities.Accounts;

namespace DeusCloud.Data.Entities.Alice
{
    public class AliceInsurance
    {
        public string eventType { get; set; }

        public long timestamp { get; set; }

        public string characterId { get; set; }

        public AliceInsuranceData data { get; set; }

        public AliceInsurance(Account user)
        {
            eventType = "change-insurance";
            timestamp = (long) (DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds);//Ticks;
            characterId = user.Login;
            data = new AliceInsuranceData(user);
        }
    }

    public class AliceInsuranceData
    {
        public string Insurance { get; set; }
        public int Level { get; set; }

        public AliceInsuranceData(Account acc)
        {
            Insurance = acc.Insurance.ToString();
            Level = acc.InsuranceLevel;
        }
    }
}