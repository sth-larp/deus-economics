namespace DeusClient.ApiCommands.Accounts.Client
{
    public sealed class RegistrationClientData
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public UserSettings Settings { get; set; }

    }

}
