using DonkeySellApi.Extra;
using DonkeySellApi.Models;

namespace DonkeySellApi.Migrations
{
    using System.Data.Entity.Migrations;

    public class Configuration : DbMigrationsConfiguration<DonkeySellApi.Models.DonkeySellContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(DonkeySellContext context)
        {
            DonkeySellDatabaseFunctions seedClass = new DonkeySellDatabaseFunctions();
            seedClass.Seed();            
        }
    }
}
