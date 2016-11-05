namespace DonkeySellApi.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class removedSocialFieldsFromProduct : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Product", "UserMail");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Product", "UserMail", c => c.String());
        }
    }
}
