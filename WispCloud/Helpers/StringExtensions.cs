using System;

namespace DeusCloud.Helpers
{
    public static class StringExtensions
    {
        public static decimal ToDecimal(this string @this)
        {
            var result = 0.0M;
            if (decimal.TryParse(@this, out result))
                return result;
            throw new InvalidCastException($"Unable to cast string\"{@this ?? string.Empty}\" as decimal");
        }
    }
}