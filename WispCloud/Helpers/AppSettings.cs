using System.Configuration;

namespace DeusCloud.Helpers
{
    public static class AppSettings
    {
        public static string Raw(string key)
        {
            return ConfigurationManager.AppSettings[key];
        }

        public static bool Is(string key)
        {
            var value = Raw(key);
            if (value == null)
                return false;

            return (value.ToLower() == "true");
        }

        public static string Url(string key)
        {
            var value = Raw(key);
            if (value == null)
                return null;

            return Raw(key).TrimEnd('/');
        }

        public static int? Int(string key)
        {
            var value = Raw(key);
            if (value == null)
                return null;

            int valueInt;
            if (int.TryParse(value, out valueInt))
                return valueInt;

            return null;
        }

    }

}