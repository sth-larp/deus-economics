namespace DeusCloud.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PaidSalary : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Accounts", "PaidSalary", c => c.Single(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Accounts", "PaidSalary");
        }
    }
}
