namespace firstProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class initial3 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Orders",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TimeToPlaceOrder = c.DateTime(nullable: false),
                        TimeToFullfillOrder = c.DateTime(nullable: false),
                        status = c.String(nullable: false),
                        userName = c.Int(nullable: false),
                        payment = c.Int(nullable: false),
                        balance = c.Int(nullable: false),
                        itemName = c.String(nullable: false),
                        amount = c.Int(nullable: false),
                        customerAddress = c.String(nullable: false),
                        restaurantName = c.String(nullable: false),
                        user_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.user_Id)
                .Index(t => t.user_Id);
            
            AddColumn("dbo.Image", "price", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Orders", "user_Id", "dbo.Users");
            DropIndex("dbo.Orders", new[] { "user_Id" });
            DropColumn("dbo.Image", "price");
            DropTable("dbo.Orders");
        }
    }
}
