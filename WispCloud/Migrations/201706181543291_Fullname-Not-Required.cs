namespace DeusCloud.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FullnameNotRequired : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Accounts", "Fullname", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Accounts", "Fullname", c => c.String(nullable: false));
        }
    }
}
