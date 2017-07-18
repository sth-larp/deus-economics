using System;
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
        Panam = 5
    }

    public static class InsuranceExtensions
    {
        public static int SetLevel(this InsuranceType type, int? level)
        {
            if (type == InsuranceType.None) return 0;
            if (level == null) return 1; 
            if (type == InsuranceType.SuperVip) return 1;
            if (type == InsuranceType.Govt) return Math.Max(1, Math.Min(2, level.Value));
            return Math.Max(1, Math.Min(3, level.Value));
        }
    }
}