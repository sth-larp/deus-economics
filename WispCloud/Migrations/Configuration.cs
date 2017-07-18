using System.Data.Entity.Migrations;
using DeusCloud.Data;

namespace DeusCloud.Migrations
{
    internal sealed class Configuration : DbMigrationsConfiguration<DeusData>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            ContextKey = "DeusCloud.Data.DeusData";
        }

        protected override void Seed(DeusData context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //
        }
    }
}
