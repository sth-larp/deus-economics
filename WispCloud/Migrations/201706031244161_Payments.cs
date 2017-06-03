namespace DeusCloud.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Payments : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Payments",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Sender = c.String(nullable: false, maxLength: 250),
                        Receiver = c.String(nullable: false, maxLength: 250),
                        LastPaid = c.DateTime(nullable: false),
                        Period = c.Time(nullable: false, precision: 7),
                        Debt = c.Single(nullable: false),
                        Amount = c.Single(nullable: false),
                        Type = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Accounts", t => t.Receiver)
                .ForeignKey("dbo.Accounts", t => t.Sender)
                .Index(t => t.Sender)
                .Index(t => t.Receiver);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Payments", "Sender", "dbo.Accounts");
            DropForeignKey("dbo.Payments", "Receiver", "dbo.Accounts");
            DropIndex("dbo.Payments", new[] { "Receiver" });
            DropIndex("dbo.Payments", new[] { "Sender" });
            DropTable("dbo.Payments");
        }
    }
}
