namespace DonkeySellApi.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class improve1 : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.Message", newName: "Messages");
            RenameTable(name: "dbo.Product", newName: "Products");
            CreateTable(
                "dbo.Categories",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CategoryName = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Cities",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CityName = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.Messages", "Read", c => c.Boolean(nullable: false));
            AddColumn("dbo.Products", "CityId", c => c.Int(nullable: false));
            AddColumn("dbo.Products", "Price", c => c.Int(nullable: false));
            AddColumn("dbo.Products", "Title", c => c.String());
            AddColumn("dbo.Products", "TradesAccepted", c => c.Boolean(nullable: false));
            AddColumn("dbo.Products", "CategoryId", c => c.Int(nullable: false));
            AddColumn("dbo.Products", "Image1", c => c.String());
            AddColumn("dbo.Products", "Image2", c => c.String());
            AddColumn("dbo.AspNetUsers", "ImageLogo", c => c.String());
            CreateIndex("dbo.Products", "CityId");
            CreateIndex("dbo.Products", "CategoryId");
            AddForeignKey("dbo.Products", "CategoryId", "dbo.Categories", "Id", cascadeDelete: true);
            AddForeignKey("dbo.Products", "CityId", "dbo.Cities", "Id", cascadeDelete: true);
            DropColumn("dbo.Products", "City");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Products", "City", c => c.String());
            DropForeignKey("dbo.Products", "CityId", "dbo.Cities");
            DropForeignKey("dbo.Products", "CategoryId", "dbo.Categories");
            DropIndex("dbo.Products", new[] { "CategoryId" });
            DropIndex("dbo.Products", new[] { "CityId" });
            DropColumn("dbo.AspNetUsers", "ImageLogo");
            DropColumn("dbo.Products", "Image2");
            DropColumn("dbo.Products", "Image1");
            DropColumn("dbo.Products", "CategoryId");
            DropColumn("dbo.Products", "TradesAccepted");
            DropColumn("dbo.Products", "Title");
            DropColumn("dbo.Products", "Price");
            DropColumn("dbo.Products", "CityId");
            DropColumn("dbo.Messages", "Read");
            DropTable("dbo.Cities");
            DropTable("dbo.Categories");
            RenameTable(name: "dbo.Products", newName: "Product");
            RenameTable(name: "dbo.Messages", newName: "Message");
        }
    }
}
