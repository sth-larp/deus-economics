namespace DeusCloud.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InsurancesLoyalties : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Loyalties",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Insurance = c.Int(nullable: false),
                        LoyalName = c.String(nullable: false, maxLength: 250),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Accounts", t => t.LoyalName)
                .Index(t => t.Insurance)
                .Index(t => t.LoyalName);
            
            AddColumn("dbo.Accounts", "Insurance", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Loyalties", "LoyalName", "dbo.Accounts");
            DropIndex("dbo.Loyalties", new[] { "LoyalName" });
            DropIndex("dbo.Loyalties", new[] { "Insurance" });
            DropColumn("dbo.Accounts", "Insurance");
            DropTable("dbo.Loyalties");
        }
    }
}
