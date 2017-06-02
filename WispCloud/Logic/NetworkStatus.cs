using System.Runtime.Serialization;

namespace WispCloud.Logic
{
    [DataContract]
    public enum NetworkStatus
    {
        None = 0,
        Pending = 1,
        Active = 2,
        Inactive = 3,
    }

}