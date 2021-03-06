﻿using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace DeusClient.ApiCommands.Accounts.Client
{
    public sealed class InstallationRolesClientData
    {
        public string Login { get; set; }
        public List<InstallationAccessRoles> Roles { get; set; }

        [JsonIgnore]
        public InstallationAccessRoles Role { get; set; }

        [OnDeserialized]
        internal void OnDeserialized(StreamingContext context)
        {
            if (Roles != null && Roles.Any())
                Role = Roles.Aggregate(InstallationAccessRoles.None, (role, next) => role |= next);
            else
                Role = InstallationAccessRoles.None;
        }

    }

}