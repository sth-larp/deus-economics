namespace DeusCloud.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class salarylevel : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Payments", "SalaryLevel", c => c.Int(nullable: false));
            DropColumn("dbo.Payments", "Debt");
            DropColumn("dbo.Payments", "Amount");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Payments", "Amount", c => c.Single(nullable: false));
            AddColumn("dbo.Payments", "Debt", c => c.Single(nullable: false));
            DropColumn("dbo.Payments", "SalaryLevel");
        }
    }
}
