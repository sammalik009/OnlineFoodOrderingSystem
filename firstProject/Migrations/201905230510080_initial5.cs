namespace firstProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class initial5 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Image", "Description", c => c.String());
            AddColumn("dbo.Orders", "comment", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Orders", "comment");
            DropColumn("dbo.Image", "Description");
        }
    }
}
