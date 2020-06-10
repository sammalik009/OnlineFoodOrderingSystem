namespace firstProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class initial6 : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Orders", "comment");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Orders", "comment", c => c.String());
        }
    }
}
