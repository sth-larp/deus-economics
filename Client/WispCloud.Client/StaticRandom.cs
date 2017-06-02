using System;

namespace WispCloudClient
{
    public static class StaticRandom
    {
        public static Random Random;

        static StaticRandom()
        {
            Random = new Random();
        }

        public static int Next(int maxValue)
        {
            return Random.Next(maxValue);
        }

    }

}
