using System.Runtime.Serialization;

namespace DeusCloud.Data.Entities.Accounts
{
    [DataContract]
    public enum AccountRole
    {
        User = 1,
        Master = 2,
        Corp = 4,
        Govt = 8,

    }

}