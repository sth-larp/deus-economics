using System;
using System.Data.Entity;

namespace DeusCloud.Data
{
    public class DeusDBInitializer : CreateDatabaseIfNotExists<DeusData>
    {
        protected override void Seed(DeusData data)
        {
            base.Seed(data);

            data.Database.ExecuteSqlCommand(TransactionalBehavior.DoNotEnsureTransaction, "ALTER DATABASE CURRENT SET AUTO_CLOSE OFF");

            var random = new Random();

            //make id for installations that hard to count
            //make unique index on hub login with nulls
        }
    }

}