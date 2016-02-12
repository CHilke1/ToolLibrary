namespace ToolLibrary.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class migration2 : DbMigration
    {
        public override void Up()
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
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Tool", t => t.Tool_Id)
                .Index(t => t.Tool_Id);
            
            AddColumn("dbo.Tool", "Barcode", c => c.Int(nullable: false));
            AddColumn("dbo.Tool", "Name", c => c.String());
            AddColumn("dbo.Tool", "Description", c => c.String());
            AddColumn("dbo.Tool", "IsCheckedOut", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Reservation", "Tool_Id", "dbo.Tool");
            DropIndex("dbo.Reservation", new[] { "Tool_Id" });
            DropColumn("dbo.Tool", "IsCheckedOut");
            DropColumn("dbo.Tool", "Description");
            DropColumn("dbo.Tool", "Name");
            DropColumn("dbo.Tool", "Barcode");
            DropTable("dbo.Reservation");
        }
    }
}
