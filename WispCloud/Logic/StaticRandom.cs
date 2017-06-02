using System;
using System.Text;

namespace WispCloud.Logic
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

            return value % maxValue;
        }

        public static string GenerateName(int length)
        {
            var shortNameBuilder = new StringBuilder();
            for (int i = 0; i < length; i++)
                shortNameBuilder.Append(NameChars[StaticRandom.Next(NameChars.Length)]);

            return shortNameBuilder.ToString();
        }

    }

}