using DeusCloud.Data.Entities.Accounts;

namespace DeusCloud.Logic.Server
{
    public sealed class InsuranceHolderServerData
    {
        public string UserLogin { get; set; }
        public string UserFullname { get; set; }
        public string CompanyLogin { get; set; }
        public int InsuranceLevel { get; set; }

        public InsuranceType Insurance;

        public InsuranceHolderServerData(string user, string company)
        {
            UserLogin = user;
            CompanyLogin = company;
            InsuranceLevel = 1;
        }
    }
}