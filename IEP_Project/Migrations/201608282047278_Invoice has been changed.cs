namespace IEP_Project.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Invoicehasbeenchanged : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Invoices", "priceForPackage", c => c.Int(nullable: false));
            DropColumn("dbo.Invoices", "pricePerToken");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Invoices", "pricePerToken", c => c.Int(nullable: false));
            DropColumn("dbo.Invoices", "priceForPackage");
        }
    }
}
