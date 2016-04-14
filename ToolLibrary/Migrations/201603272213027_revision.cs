namespace ToolLibrary.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class revision : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.BasketItem", "BasketID", "dbo.Basket");
            DropForeignKey("dbo.BasketItem", "ToolID", "dbo.Tool");
            DropForeignKey("dbo.Reservation", "Tool_Id", "dbo.Tool");
            DropIndex("dbo.Basket", new[] { "SessionId" });
            DropIndex("dbo.BasketItem", new[] { "BasketID" });
            DropIndex("dbo.BasketItem", new[] { "ToolID" });
            DropIndex("dbo.Reservation", new[] { "Tool_Id" });
            DropTable("dbo.Basket");
            DropTable("dbo.BasketItem");
            DropTable("dbo.Reservation");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.Reservation",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DateReserved = c.DateTime(nullable: false),
                        DueDate = c.DateTime(nullable: false),
                        UserID = c.Int(nullable: false),
                        Tool_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.BasketItem",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        BasketID = c.Int(nullable: false),
                        ToolID = c.Int(nullable: false),
                        Quantity = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Basket",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SessionId = c.String(maxLength: 255),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateIndex("dbo.Reservation", "Tool_Id");
            CreateIndex("dbo.BasketItem", "ToolID");
            CreateIndex("dbo.BasketItem", "BasketID");
            CreateIndex("dbo.Basket", "SessionId", unique: true);
            AddForeignKey("dbo.Reservation", "Tool_Id", "dbo.Tool", "Id");
            AddForeignKey("dbo.BasketItem", "ToolID", "dbo.Tool", "Id", cascadeDelete: true);
            AddForeignKey("dbo.BasketItem", "BasketID", "dbo.Basket", "Id", cascadeDelete: true);
        }
    }
}
