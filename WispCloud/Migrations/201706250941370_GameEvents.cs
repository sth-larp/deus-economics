namespace DeusCloud.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class GameEvents : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.GameEvents",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        User = c.String(nullable: false, maxLength: 250),
                        Time = c.DateTime(nullable: false),
                        Type = c.Int(nullable: false),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.User);
            
        }
        
        public override void Down()
        {
            DropIndex("dbo.GameEvents", new[] { "User" });
            DropTable("dbo.GameEvents");
        }
    }
}
