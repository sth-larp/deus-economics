using System.Runtime.Serialization;

namespace DeusCloud.Data.Entities.Taxes
{
    [DataContract]
    public enum TaxType
    {
        Transaction = 1,
        Corporate = 2,
        Tavern = 3
    }
}