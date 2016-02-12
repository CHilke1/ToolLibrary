namespace ToolLibrary.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class seed2 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Tool", "Barcode", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Tool", "Barcode", c => c.Int(nullable: false));
        }
    }
}
