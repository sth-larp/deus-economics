﻿using System.Collections.Generic;
using DeusCloud.Data.Entities.Accounts;
using DeusCloud.Exceptions;
using DeusCloud.Logic.CommonBase;

namespace DeusCloud.Logic.Client
{
    public sealed class AccPropertyClientData : BaseModel
    {
        public string Login { get; set; }
        public List<AccountRole> Roles { get; set; }
        public AccountStatus? Status { get; set; }
        public InsuranceType? Insurance { get; set; }
        public uint? InsuranceLevel { get; set; }

        public override void Validate()
        {
            Try.NotEmpty(Login, $"{nameof(Login)} cant be empty.");
            //Try.Condition(Roles != null && Roles.Any(x => x != AccountRole.None), $"{nameof(Roles)} cant be empty.");
        }

    }

}