namespace DeusCloud.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Emails : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Accounts", "Email", c => c.String(maxLength: 80, defaultValueSql: "SUBSTRING(CONVERT(varchar(40), NEWID()), 0, 9) With Values"));
            CreateIndex("dbo.Accounts", "Email", unique: true);
        }
        
        public override void Down()
        {
            DropIndex("dbo.Accounts", new[] { "Email" });
            DropColumn("dbo.Accounts", "Email");
        }
    }
}
