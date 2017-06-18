using System.Runtime.Serialization;

namespace DeusCloud.Data.Entities.Constants
{
    [DataContract]
    public enum ConstantType
    {
        Transaction = 1,
        Corporate = 2,
        TavernTax = 3
    }
}