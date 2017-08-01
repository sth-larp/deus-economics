namespace DeusCloud.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class constantsrefactor : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("dbo.Constants");
            AddColumn("dbo.Constants", "Name", c => c.String(nullable: false, maxLength: 128));
            AddColumn("dbo.Constants", "Value", c => c.Single(nullable: false));
            AddPrimaryKey("dbo.Constants", "Name");
            DropColumn("dbo.Constants", "Type");
            DropColumn("dbo.Constants", "PercentValue");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Constants", "PercentValue", c => c.Single(nullable: false));
            AddColumn("dbo.Constants", "Type", c => c.Int(nullable: false));
            DropPrimaryKey("dbo.Constants");
            DropColumn("dbo.Constants", "Value");
            DropColumn("dbo.Constants", "Name");
            AddPrimaryKey("dbo.Constants", "Type");
        }
    }
}
