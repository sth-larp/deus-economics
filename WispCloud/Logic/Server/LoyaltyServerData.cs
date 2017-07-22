using DeusCloud.Data.Entities.Accounts;

namespace DeusCloud.Logic.Server
{
    public class LoyaltyServerData
    {
        public int Id { get; set; }

        public InsuranceType Insurance { get; set; }

        public int MinLevel { get; set; }
        public string LoyalName { get; set; }

        public string LoyalFullName { get; set; }
        
        public LoyaltyServerData(int id, InsuranceType ins, string name, string fullname)
        {
            Id = id;
            Insurance = ins;
            LoyalName = name;
            LoyalFullName = fullname;
            MinLevel = 1;
        }
    }
}