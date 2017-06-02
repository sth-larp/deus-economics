namespace DeusClient.ApiCommands.Accounts.Client
{
    public sealed class Authorization
    {
        public string access_token { get; set; }
        public string token_type { get; set; }
        public ulong expires_in { get; set; }
    }
}
