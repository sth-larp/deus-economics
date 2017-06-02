namespace DeusClient.ApiCommands.Accounts.Client
{
    public enum InstallationAccessRoles
    {
        None = 0,
        Hub = 1 << 0,
        Administrator = 1 << 1,
        User = 1 << 2,
        SmartUser = 1 << 3,
        Installer = 1 << 4,

    }

}
