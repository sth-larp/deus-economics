using System.Runtime.Serialization;

namespace DeusCloud.Data.Entities.Accounts
{
    [DataContract]
    public enum InsuranceType
    {
        None = 0,
        Govt1 = 1,
        Govt2 = 2,
        SuperVip = 3,
        JJ = 4,
        Serenity = 5,
        Corp3 = 6
    }
}