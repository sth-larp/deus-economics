using System.Configuration;

namespace WispCloud
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

    }

}