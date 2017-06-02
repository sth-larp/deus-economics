using System;
using System.Runtime.Serialization;

namespace DeusCloud.Data.Entities.Transactions
{
    [DataContract]
    [Flags]
    public enum TransactionType
    {
        None = 0,
        Normal = 1,
        Tax = 2,
        Hidden = 4,
    }
}