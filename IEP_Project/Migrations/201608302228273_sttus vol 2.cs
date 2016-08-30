namespace IEP_Project.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class sttusvol2 : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Auctions", "status");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Auctions", "status", c => c.Int(nullable: false));
        }
    }
}
