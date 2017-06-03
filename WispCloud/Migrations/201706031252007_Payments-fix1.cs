namespace DeusCloud.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Paymentsfix1 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Payments", "LastPaid", c => c.DateTime());
            AlterColumn("dbo.Payments", "Period", c => c.Time(precision: 7));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Payments", "Period", c => c.Time(nullable: false, precision: 7));
            AlterColumn("dbo.Payments", "LastPaid", c => c.DateTime(nullable: false));
        }
    }
}
