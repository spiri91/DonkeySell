using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using DonkeySellApi.Models.DatabaseModels;
using DonkeySellApi.Models.Shared;
using Microsoft.AspNet.Identity.EntityFramework;

namespace DonkeySellApi.Models
{
    public class DonkeySellContext : IdentityDbContext<DonkeySellUser>
    {
        public DonkeySellContext() : base("DonkeySellDb")
        {
            
        }

        public static DonkeySellContext Create()
        {
            return new DonkeySellContext();
        }

        public DbSet<Message> Messages { get; set; }
        public DbSet<Product> Products { get; set; }

        public DbSet<Category> Categories { get; set; }

        public DbSet<City> Cities { get; set; }

        public DbSet<UsersFavoriteProducts> UsersFavoriteProducts { get; set; }

        public DbSet<Image> Images { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            Database.SetInitializer<DonkeySellContext>(null);

            base.OnModelCreating(modelBuilder);

            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
           
            modelBuilder.Entity<DonkeySellUser>().ToTable("AspNetUsers");
        }
    }
}