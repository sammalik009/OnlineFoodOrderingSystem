namespace firstProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class initial2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Restaurant", "OwnerName", c => c.String(nullable: false));
            AlterColumn("dbo.Restaurant", "Name", c => c.String(nullable: false));
            AlterColumn("dbo.Restaurant", "Location", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Restaurant", "Location", c => c.String());
            AlterColumn("dbo.Restaurant", "Name", c => c.String());
            DropColumn("dbo.Restaurant", "OwnerName");
        }
    }
}
