namespace firstProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class initial4 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Restaurant", "RestaurantTitle", c => c.String(nullable: false));
            DropColumn("dbo.Restaurant", "OwnerName");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Restaurant", "OwnerName", c => c.String(nullable: false));
            DropColumn("dbo.Restaurant", "RestaurantTitle");
        }
    }
}
