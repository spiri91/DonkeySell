namespace DonkeySellApi.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedAlertsAndMeetingPoints : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Alert",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(maxLength: 128),
                        ProductName = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId);
            
            AddColumn("dbo.Product", "MeetingPoint", c => c.String());
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Alert", "UserId", "dbo.AspNetUsers");
            DropIndex("dbo.Alert", new[] { "UserId" });
            DropColumn("dbo.Product", "MeetingPoint");
            DropTable("dbo.Alert");
        }
    }
}
