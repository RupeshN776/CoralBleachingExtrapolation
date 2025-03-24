using CoralBleachingExtrapolation.Data;
using CoralBleachingExtrapolation.Models;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// 03-03-2025  1.0   Keelin   Connecting data from GBR to the model  
/// </summary>
/// 

namespace CoralBleachingExtrapolation.Data
{
    public class ApplicationDbContextGBR : DbContext
    {
        public ApplicationDbContextGBR(DbContextOptions<ApplicationDbContextGBR> options) : base(options)
        {

        }
        public DbSet<GBRCoralPoint> tbl_GBRCoralPoint { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure NetTopologySuite for spatial data
            modelBuilder.Entity<GBRCoralPoint>()
                .Property(w => w.Point)
                .HasColumnType("geography") // Ensure it maps to a spatial column
                .HasDefaultValueSql("geography::STGeomFromText('POINT EMPTY', 4326)");
        }
    }
}

