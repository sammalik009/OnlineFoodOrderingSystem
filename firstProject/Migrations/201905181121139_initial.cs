namespace firstProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class initial : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Image", "RestaurantName", c => c.String(nullable: false));
            AddColumn("dbo.Image", "Type", c => c.String());
            AddColumn("dbo.Image", "restaurant_Id", c => c.Int());
            AlterColumn("dbo.Image", "Title", c => c.String(nullable: false));
            CreateIndex("dbo.Image", "restaurant_Id");
            AddForeignKey("dbo.Image", "restaurant_Id", "dbo.Restaurant", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Image", "restaurant_Id", "dbo.Restaurant");
            DropIndex("dbo.Image", new[] { "restaurant_Id" });
            AlterColumn("dbo.Image", "Title", c => c.String());
            DropColumn("dbo.Image", "restaurant_Id");
            DropColumn("dbo.Image", "Type");
            DropColumn("dbo.Image", "RestaurantName");
        }
    }
}
