using System;
using System.Text;

namespace DeusCloud.Logic.CommonBase
{
    public static class StaticRandom
    {
        public static string NameChars { get; }
        public static Random Random { get; }

        static StaticRandom()
        {
            NameChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            Random = new Random();
        }

        public static int Next()
        {
            return Random.Next();
        }
        public static int Next(int maxValue)
        {
            return Random.Next(maxValue);
        }

        public static long NextLong()
        {
            return NextLong(long.MaxValue);
        }
        public static long NextLong(long maxValue)
        {
            byte[] buf = new byte[8];
            Random.NextBytes(buf);
            long value = BitConverter.ToInt64(buf, 0);
            value %= maxValue;
            if (value < 0)
                value *= -1;

            return value;
        }

        public static string GenerateString(int length)
        {
            var nameBuilder = new StringBuilder();
            for (int i = 0; i < length; i++)
                nameBuilder.Append(NameChars[StaticRandom.Next(NameChars.Length)]);

            return nameBuilder.ToString();
        }

    }

}