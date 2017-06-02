using System.Runtime.Serialization;

namespace DeusCloud.Data.Entities.Accounts
{
    [DataContract]
    public enum AccountStatus
    {
        Active = 1,
        Blocked = 2
    }
}