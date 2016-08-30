namespace IEP_Project.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Addedusertoauction : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Auctions", "lastBidder_Id", c => c.String(maxLength: 128));
            CreateIndex("dbo.Auctions", "lastBidder_Id");
            AddForeignKey("dbo.Auctions", "lastBidder_Id", "dbo.AspNetUsers", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Auctions", "lastBidder_Id", "dbo.AspNetUsers");
            DropIndex("dbo.Auctions", new[] { "lastBidder_Id" });
            DropColumn("dbo.Auctions", "lastBidder_Id");
        }
    }
}
