namespace DeusCloud.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class First : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AccountAccesses",
                c => new
                    {
                        Slave = c.String(nullable: false, maxLength: 250),
                        Master = c.String(nullable: false, maxLength: 250),
                        Role = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Slave, t.Master })
                .ForeignKey("dbo.Accounts", t => t.Master)
                .ForeignKey("dbo.Accounts", t => t.Slave)
                .Index(t => t.Slave)
                .Index(t => t.Master);
            
            CreateTable(
                "dbo.Accounts",
                c => new
                    {
                        Login = c.String(nullable: false, maxLength: 250),
                        SettingsJson = c.String(),
                        PasswordHash = c.String(nullable: false),
                        TokenSalt = c.Guid(nullable: false),
                        Role = c.Int(nullable: false),
                        Cash = c.Single(nullable: false),
                        Status = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Login);
            
            CreateTable(
                "dbo.Payments",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Sender = c.String(nullable: false, maxLength: 250),
                        Receiver = c.String(nullable: false, maxLength: 250),
                        LastPaid = c.DateTime(),
                        Period = c.Time(precision: 7),
                        Debt = c.Single(nullable: false),
                        Amount = c.Single(nullable: false),
                        Type = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Accounts", t => t.Receiver)
                .ForeignKey("dbo.Accounts", t => t.Sender)
                .Index(t => t.Sender)
                .Index(t => t.Receiver);
            
            CreateTable(
                "dbo.Taxes",
                c => new
                    {
                        Type = c.Int(nullable: false),
                        PercentValue = c.Single(nullable: false),
                        Description = c.String(maxLength: 250),
                    })
                .PrimaryKey(t => t.Type);
            
            CreateTable(
                "dbo.Transactions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Sender = c.String(nullable: false, maxLength: 250),
                        Receiver = c.String(nullable: false, maxLength: 250),
                        Time = c.DateTime(nullable: false),
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
            DropForeignKey("dbo.Transactions", "Sender", "dbo.Accounts");
            DropForeignKey("dbo.Transactions", "Receiver", "dbo.Accounts");
            DropForeignKey("dbo.Payments", "Sender", "dbo.Accounts");
            DropForeignKey("dbo.Payments", "Receiver", "dbo.Accounts");
            DropForeignKey("dbo.AccountAccesses", "Slave", "dbo.Accounts");
            DropForeignKey("dbo.AccountAccesses", "Master", "dbo.Accounts");
            DropIndex("dbo.Transactions", new[] { "Receiver" });
            DropIndex("dbo.Transactions", new[] { "Sender" });
            DropIndex("dbo.Payments", new[] { "Receiver" });
            DropIndex("dbo.Payments", new[] { "Sender" });
            DropIndex("dbo.AccountAccesses", new[] { "Master" });
            DropIndex("dbo.AccountAccesses", new[] { "Slave" });
            DropTable("dbo.Transactions");
            DropTable("dbo.Taxes");
            DropTable("dbo.Payments");
            DropTable("dbo.Accounts");
            DropTable("dbo.AccountAccesses");
        }
    }
}
