namespace ToolLibrary.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Basket",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SessionID = c.String(maxLength: 255),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.SessionID, unique: true);
            
            CreateTable(
                "dbo.BasketItem",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        BasketID = c.Int(nullable: false),
                        ToolID = c.Int(nullable: false),
                        Quantity = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Basket", t => t.BasketID, cascadeDelete: true)
                .ForeignKey("dbo.Tool", t => t.ToolID, cascadeDelete: true)
                .Index(t => t.BasketID)
                .Index(t => t.ToolID);
            
            CreateTable(
                "dbo.Tool",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Manufacturer = c.String(),
                        AdditionalDescription = c.String(),
                        ImageUrl = c.String(),
                        Type = c.Int(nullable: false),
                        Category_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Category", t => t.Category_Id)
                .Index(t => t.Category_Id);
            
            CreateTable(
                "dbo.Category",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Rental",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CheckedOut = c.DateTime(nullable: false),
                        DueDate = c.DateTime(nullable: false),
                        UserID = c.Int(nullable: false),
                        Tool_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Tool", t => t.Tool_Id)
                .Index(t => t.Tool_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Rental", "Tool_Id", "dbo.Tool");
            DropForeignKey("dbo.BasketItem", "ToolID", "dbo.Tool");
            DropForeignKey("dbo.Tool", "Category_Id", "dbo.Category");
            DropForeignKey("dbo.BasketItem", "BasketID", "dbo.Basket");
            DropIndex("dbo.Rental", new[] { "Tool_Id" });
            DropIndex("dbo.Tool", new[] { "Category_Id" });
            DropIndex("dbo.BasketItem", new[] { "ToolID" });
            DropIndex("dbo.BasketItem", new[] { "BasketID" });
            DropIndex("dbo.Basket", new[] { "SessionID" });
            DropTable("dbo.Rental");
            DropTable("dbo.Category");
            DropTable("dbo.Tool");
            DropTable("dbo.BasketItem");
            DropTable("dbo.Basket");
        }
    }
}
