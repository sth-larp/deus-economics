namespace DeusCloud.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Index : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Accounts", "Index", c => c.Int(nullable: false, defaultValue: 0));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Accounts", "Index");
        }
    }
}
