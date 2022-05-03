namespace BeerManagement.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddObjectId : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Stocks", "Beer_BeerId", "dbo.Beers");
            DropForeignKey("dbo.Stocks", "Wholesaler_WholesalerId", "dbo.Wholesalers");
            DropIndex("dbo.Stocks", new[] { "Beer_BeerId" });
            DropIndex("dbo.Stocks", new[] { "Wholesaler_WholesalerId" });
            RenameColumn(table: "dbo.Stocks", name: "Beer_BeerId", newName: "BeerId");
            RenameColumn(table: "dbo.Stocks", name: "Wholesaler_WholesalerId", newName: "WholesalerId");
            AlterColumn("dbo.Stocks", "BeerId", c => c.Guid(nullable: false));
            AlterColumn("dbo.Stocks", "WholesalerId", c => c.Guid(nullable: false));
            CreateIndex("dbo.Stocks", "WholesalerId");
            CreateIndex("dbo.Stocks", "BeerId");
            AddForeignKey("dbo.Stocks", "BeerId", "dbo.Beers", "BeerId", cascadeDelete: true);
            AddForeignKey("dbo.Stocks", "WholesalerId", "dbo.Wholesalers", "WholesalerId", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Stocks", "WholesalerId", "dbo.Wholesalers");
            DropForeignKey("dbo.Stocks", "BeerId", "dbo.Beers");
            DropIndex("dbo.Stocks", new[] { "BeerId" });
            DropIndex("dbo.Stocks", new[] { "WholesalerId" });
            AlterColumn("dbo.Stocks", "WholesalerId", c => c.Guid());
            AlterColumn("dbo.Stocks", "BeerId", c => c.Guid());
            RenameColumn(table: "dbo.Stocks", name: "WholesalerId", newName: "Wholesaler_WholesalerId");
            RenameColumn(table: "dbo.Stocks", name: "BeerId", newName: "Beer_BeerId");
            CreateIndex("dbo.Stocks", "Wholesaler_WholesalerId");
            CreateIndex("dbo.Stocks", "Beer_BeerId");
            AddForeignKey("dbo.Stocks", "Wholesaler_WholesalerId", "dbo.Wholesalers", "WholesalerId");
            AddForeignKey("dbo.Stocks", "Beer_BeerId", "dbo.Beers", "BeerId");
        }
    }
}
