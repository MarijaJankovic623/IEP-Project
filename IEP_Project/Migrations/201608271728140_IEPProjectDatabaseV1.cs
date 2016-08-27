namespace IEP_Project.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class IEPProjectDatabaseV1 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Auctions",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        productName = c.String(),
                        Image = c.Binary(),
                        initialPrice = c.Int(nullable: false),
                        currentPriceRaise = c.Int(nullable: false),
                        status = c.Int(nullable: false),
                        duration = c.Int(nullable: false),
                        creatingDateTime = c.DateTime(nullable: false),
                        startingDateTime = c.DateTime(nullable: false),
                        finishingDateTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.Bids",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        sentTime = c.DateTime(nullable: false),
                        auction_ID = c.Int(),
                        user_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Auctions", t => t.auction_ID)
                .ForeignKey("dbo.AspNetUsers", t => t.user_Id)
                .Index(t => t.auction_ID)
                .Index(t => t.user_Id);
            
            CreateTable(
                "dbo.Invoices",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        tokenNumber = c.Int(nullable: false),
                        pricePerToken = c.Int(nullable: false),
                        status = c.Int(nullable: false),
                        creatingDateTime = c.DateTime(nullable: false),
                        user_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.AspNetUsers", t => t.user_Id)
                .Index(t => t.user_Id);
            
            AddColumn("dbo.AspNetUsers", "name", c => c.String());
            AddColumn("dbo.AspNetUsers", "lastname", c => c.String());
            AddColumn("dbo.AspNetUsers", "tokenNumber", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Invoices", "user_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.Bids", "user_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.Bids", "auction_ID", "dbo.Auctions");
            DropIndex("dbo.Invoices", new[] { "user_Id" });
            DropIndex("dbo.Bids", new[] { "user_Id" });
            DropIndex("dbo.Bids", new[] { "auction_ID" });
            DropColumn("dbo.AspNetUsers", "tokenNumber");
            DropColumn("dbo.AspNetUsers", "lastname");
            DropColumn("dbo.AspNetUsers", "name");
            DropTable("dbo.Invoices");
            DropTable("dbo.Bids");
            DropTable("dbo.Auctions");
        }
    }
}
