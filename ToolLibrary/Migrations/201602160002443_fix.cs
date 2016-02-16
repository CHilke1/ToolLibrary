namespace ToolLibrary.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fix : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Rental", "UserID", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Rental", "UserID", c => c.Int(nullable: false));
        }
    }
}
