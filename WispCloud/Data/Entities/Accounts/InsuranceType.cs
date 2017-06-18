using System.Runtime.Serialization;

namespace DeusCloud.Data.Entities.Accounts
{
    [DataContract]
    public enum InsuranceType
    {
        None = 0,
        Govt = 1,
        SuperVip = 2,
        JJ = 3,
        Serenity = 4,
        Corp3 = 5
    }
}