﻿using DeusCloud.Data.Entities;

namespace DeusCloud.Helpers
{
    public static class Constants
    {
        public static class WispCloud
        {
            public const string DefaultConnectionStringName = "DeusMaster";
            public const string NotEnoughRightsMessageText = "You are not allowed to perform this operation;";
            public const string InstallationIsDisabledMessageText = "Cant find installation;";
            public const string MessageBusEmptyTopicsList = "List of subscribed topics cannot be null. For All topics subscription pass new string{\"\"} as argument";
            public static AccountAccessRoles AccountAdminRole = AccountAccessRoles.Installer | AccountAccessRoles.Administrator;
            public const int MaxGroupsCount = 8;
            public const int MinimalHubHeartbeat = 1000;
            public const int MaximalHubHeartbeat = 60 * 1000;
        }
        public static class AppSettings
        {
            public const string HubInactiveTimeout = "Hub.InactiveTimeout";
            public const string HubInactiveHeartbeats = "Hub.InactiveHeartbeats";
            public const string DefaultHeartbeatPeriod = "Hub.DefaultHeartbeatPeriod";

            public static class Engine
            {
                public static class MessageBus
                {
                    public const string Subscription = "Engine.MessageBus.Subscription";
                    public const string Publishing = "Engine.MessageBus.Publishing";
                }
            }
        }

        public static class WispEngine
        {
            public const int Tick = 50; // msec
        }
    }
}