namespace DonkeySellApi.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class setArrayOnImages : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Product", "Image1");
            DropColumn("dbo.Product", "Image2");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Product", "Image2", c => c.String());
            AddColumn("dbo.Product", "Image1", c => c.String());
        }
    }
}
