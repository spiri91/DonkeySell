namespace DonkeySellApi.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedConnectionId : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "ConnectionId", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "ConnectionId");
        }
    }
}
