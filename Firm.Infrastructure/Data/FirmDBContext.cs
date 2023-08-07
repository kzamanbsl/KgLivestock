using Firm.Core.EntityModel;
using Firm.Infrastructure.Auth;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Firm.Infrastructure.Data
{
    public class FirmDBContext : IdentityDbContext<ApplicationUser>
    {
        public FirmDBContext(DbContextOptions<FirmDBContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            FixedData.SeedData(builder);
            base.OnModelCreating(builder);
        }

        public virtual DbSet<Cow> Cows { get; set; }
        public virtual DbSet<Breed> Breeds { get; set; }
        public virtual DbSet<Doctor> Doctors { get; set; }
        public virtual DbSet<Treatment> Treatments { get; set; }
        public virtual DbSet<Vaccine> Vaccines { get; set; }
        public virtual DbSet<MilkMonitor> MilkMonitors { get; set; }
        public virtual DbSet<FeedCategory> FeedCategories { get; set; }
        public virtual DbSet<FeedEntry> FeedEntries { get; set; }
        public virtual DbSet<FeedCurrentStock> FeedCurrentStocks { get; set; }
        public virtual DbSet<FeedConsumptionBulk> FeedConsumptionBulks { get; set; }
        public virtual DbSet<FeedConsumptionCowWise> FeedConsumptionCowWises { get; set; }
        public virtual DbSet<SaleableItem> SaleableItems { get; set; }

    }
}
