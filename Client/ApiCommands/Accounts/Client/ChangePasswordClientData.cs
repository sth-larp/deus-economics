﻿namespace DeusClient.ApiCommands.Accounts.Client
{
    public sealed class ChangePasswordClientData
    {
        public string Login { get; set; }
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }

    }

}