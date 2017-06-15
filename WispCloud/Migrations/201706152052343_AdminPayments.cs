namespace DeusCloud.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AdminPayments : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.Payments", name: "Sender", newName: "Employer");
            AddColumn("dbo.Transactions", "Comment", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Transactions", "Comment");
            RenameColumn(table: "dbo.Payments", name: "Employer", newName: "Sender");
        }
    }
}
