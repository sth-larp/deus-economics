namespace DeusCloud.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Insurancelevel : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Accounts", "InsuranceLevel", c => c.Int(nullable: false, defaultValue: 1));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Accounts", "InsuranceLevel");
        }
    }
}
