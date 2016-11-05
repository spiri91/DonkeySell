namespace DonkeySellApi.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class improve1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Product", "Free", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Product", "Free");
        }
    }
}
