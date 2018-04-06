using DeusCloud.Data.Entities.Accounts;
using System;
using System.Data.Entity;

namespace DeusCloud.Data
{
    public class DeusDBInitializer : CreateDatabaseIfNotExists<DeusData>
    {
        protected override void Seed(DeusData data)
        {
            base.Seed(data);

            data.Accounts.Add(new Account("admin", passwordHash: "AAWAAcy/xdBPTCL+nD+2Mcbq7SgrR1l5L9OkSXNZ+bQhCv8XierEz1jfpPRq7/+dgA=="));

            var random = new Random();

            //make id for installations that hard to count
            //make unique index on hub login with nulls
        }
    }

}