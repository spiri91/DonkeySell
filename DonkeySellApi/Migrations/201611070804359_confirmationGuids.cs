namespace DonkeySellApi.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class confirmationGuids : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "ConfirmationGuid", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "ConfirmationGuid");
        }
    }
}
