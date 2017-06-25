namespace DeusCloud.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FieldRename : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Accounts", "InsurancePoints", c => c.Int(nullable: false));
            DropColumn("dbo.Accounts", "IndexSpent");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Accounts", "IndexSpent", c => c.Int(nullable: false));
            DropColumn("dbo.Accounts", "InsurancePoints");
        }
    }
}
