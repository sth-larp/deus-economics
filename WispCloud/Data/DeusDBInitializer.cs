using System;
using System.Data.Entity;
using DeusCloud.Data;
using DeusCloud.Data.Entities;

namespace WispCloud.Data
{
    public class DeusDBInitializer : CreateDatabaseIfNotExists<DeusData>
    {
        protected override void Seed(DeusData data)
        {
            base.Seed(data);

            data.Database.ExecuteSqlCommand(TransactionalBehavior.DoNotEnsureTransaction, "ALTER DATABASE CURRENT SET AUTO_CLOSE OFF");

            var random = new Random();

            //make id for installations that hard to count
            data.Database.ExecuteSqlCommand($"CREATE SEQUENCE {Installation.SequenceName} AS bigint START WITH {random.Next(1000000, 1000000000)} INCREMENT BY {random.Next(100, 1000)}");

            //make unique index on hub login with nulls
        }

    }

}