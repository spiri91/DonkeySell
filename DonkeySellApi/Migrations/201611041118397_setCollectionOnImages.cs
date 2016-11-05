namespace DonkeySellApi.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class setCollectionOnImages : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Image",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Value = c.String(),
                        Product_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Product", t => t.Product_Id)
                .Index(t => t.Product_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Image", "Product_Id", "dbo.Product");
            DropIndex("dbo.Image", new[] { "Product_Id" });
            DropTable("dbo.Image");
        }
    }
}
