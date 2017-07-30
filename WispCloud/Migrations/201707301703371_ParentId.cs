namespace DeusCloud.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ParentId : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Accounts", "ParentID", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Accounts", "ParentID");
        }
    }
}
