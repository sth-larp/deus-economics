namespace WispCloudClient.ApiTypes
{
    public sealed class Account
    {
        public string Login { get; set; }
        public AccountRoles Role { get; set; }
        public UserSettings Settings { get; set; }

    }

}
