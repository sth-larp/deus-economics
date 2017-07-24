namespace DeusCloud.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Alias : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Accounts", "Alias", c => c.String(maxLength: 20, defaultValueSql: "SUBSTRING(CONVERT(varchar(40), NEWID()), 0, 6) With Values"));
            CreateIndex("dbo.Accounts", "Alias", unique: true);
        }
        
        public override void Down()
        {
            DropIndex("dbo.Accounts", new[] { "Alias" });
            DropColumn("dbo.Accounts", "Alias");
        }
    }
}
