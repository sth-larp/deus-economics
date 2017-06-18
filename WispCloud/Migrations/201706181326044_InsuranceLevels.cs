namespace DeusCloud.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InsuranceLevels : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Accounts", "Fullname", c => c.String(nullable: false));
            AddColumn("dbo.Loyalties", "MinLevel", c => c.Int(nullable: false, defaultValue: 1));
            DropColumn("dbo.Accounts", "SettingsJson");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Accounts", "SettingsJson", c => c.String());
            DropColumn("dbo.Loyalties", "MinLevel");
            DropColumn("dbo.Accounts", "Fullname");
        }
    }
}
