using System.Configuration;

namespace DeusCloud.Helpers
{
    public static class ConnectionStrings
    {
        public static string Get(string name)
        {
            var connectionStringSettings = ConfigurationManager.ConnectionStrings[name];
            if (connectionStringSettings != null)
                return connectionStringSettings.ConnectionString;

            return null;
        }

    }

}