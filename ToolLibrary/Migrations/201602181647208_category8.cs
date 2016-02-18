namespace ToolLibrary.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class category8 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Tool", "CategoryId", "dbo.Category");
            DropPrimaryKey("dbo.Category");
            AddColumn("dbo.Category", "CategoryId", c => c.Int(nullable: false, identity: true));
            AddPrimaryKey("dbo.Category", "CategoryId");
            AddForeignKey("dbo.Tool", "CategoryId", "dbo.Category", "CategoryId", cascadeDelete: true);
            DropColumn("dbo.Category", "Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Category", "Id", c => c.Int(nullable: false, identity: true));
            DropForeignKey("dbo.Tool", "CategoryId", "dbo.Category");
            DropPrimaryKey("dbo.Category");
            DropColumn("dbo.Category", "CategoryId");
            AddPrimaryKey("dbo.Category", "Id");
            AddForeignKey("dbo.Tool", "CategoryId", "dbo.Category", "Id", cascadeDelete: true);
        }
    }
}
