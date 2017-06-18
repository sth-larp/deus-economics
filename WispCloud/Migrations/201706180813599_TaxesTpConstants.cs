namespace DeusCloud.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TaxesTpConstants : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.Taxes", newName: "Constants");
        }
        
        public override void Down()
        {
            RenameTable(name: "dbo.Constants", newName: "Taxes");
        }
    }
}
