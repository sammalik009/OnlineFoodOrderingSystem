namespace firstProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class initial8 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CartItem",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        userName = c.String(nullable: false),
                        payment = c.Int(nullable: false),
                        itemName = c.String(nullable: false),
                        amount = c.Int(nullable: false),
                        customerAddress = c.String(nullable: false),
                        restaurantName = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.CartItem");
        }
    }
}
