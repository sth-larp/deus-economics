namespace DeusCloud.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Taxes : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Taxes",
                c => new
                    {
                        Type = c.Int(nullable: false),
                        PercentValue = c.Single(nullable: false),
                        Description = c.String(maxLength: 250),
                    })
                .PrimaryKey(t => t.Type);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Taxes");
        }
    }
}
