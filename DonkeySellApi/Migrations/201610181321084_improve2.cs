namespace DonkeySellApi.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class improve2 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Product", "UserName", "dbo.AspNetUsers");
            DropIndex("dbo.Product", new[] { "UserName" });
            AddColumn("dbo.Product", "UserId", c => c.String(maxLength: 128));
            AlterColumn("dbo.Product", "UserName", c => c.String());
            CreateIndex("dbo.Product", "UserId");
            AddForeignKey("dbo.Product", "UserId", "dbo.AspNetUsers", "UserId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Product", "UserId", "dbo.AspNetUsers");
            DropIndex("dbo.Product", new[] { "UserId" });
            AlterColumn("dbo.Product", "UserName", c => c.String(maxLength: 128));
            DropColumn("dbo.Product", "UserId");
            CreateIndex("dbo.Product", "UserName");
            AddForeignKey("dbo.Product", "UserName", "dbo.AspNetUsers", "UserId");
        }
    }
}
