namespace BeerManagement.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Beers",
                c => new
                    {
                        BeerId = c.Guid(nullable: false),
                        Name = c.String(),
                        AlcoholDegree = c.String(),
                        Price = c.Double(nullable: false),
                        Brewery_BreweryId = c.Guid(),
                    })
                .PrimaryKey(t => t.BeerId)
                .ForeignKey("dbo.Breweries", t => t.Brewery_BreweryId)
                .Index(t => t.Brewery_BreweryId);
            
            CreateTable(
                "dbo.Breweries",
                c => new
                    {
                        BreweryId = c.Guid(nullable: false),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.BreweryId);
            
            CreateTable(
                "dbo.Stocks",
                c => new
                    {
                        StockId = c.Guid(nullable: false),
                        Quantity = c.Int(nullable: false),
                        Beer_BeerId = c.Guid(),
                        Wholesaler_WholesalerId = c.Guid(),
                    })
                .PrimaryKey(t => t.StockId)
                .ForeignKey("dbo.Beers", t => t.Beer_BeerId)
                .ForeignKey("dbo.Wholesalers", t => t.Wholesaler_WholesalerId)
                .Index(t => t.Beer_BeerId)
                .Index(t => t.Wholesaler_WholesalerId);
            
            CreateTable(
                "dbo.Wholesalers",
                c => new
                    {
                        WholesalerId = c.Guid(nullable: false),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.WholesalerId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Stocks", "Wholesaler_WholesalerId", "dbo.Wholesalers");
            DropForeignKey("dbo.Stocks", "Beer_BeerId", "dbo.Beers");
            DropForeignKey("dbo.Beers", "Brewery_BreweryId", "dbo.Breweries");
            DropIndex("dbo.Stocks", new[] { "Wholesaler_WholesalerId" });
            DropIndex("dbo.Stocks", new[] { "Beer_BeerId" });
            DropIndex("dbo.Beers", new[] { "Brewery_BreweryId" });
            DropTable("dbo.Wholesalers");
            DropTable("dbo.Stocks");
            DropTable("dbo.Breweries");
            DropTable("dbo.Beers");
        }
    }
}
