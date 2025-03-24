using CoralBleachingExtrapolation.Models;
using Microsoft.EntityFrameworkCore;


/// <summary>
/// 02-29-2025  1.0   Ben   Connecting data from WorldCoral to the model  
/// </summary>
namespace CoralBleachingExtrapolation.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) 
        {
        
        }

        public DbSet<WorldCoral> tbl_GlobalCoralPolygon { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure NetTopologySuite for spatial data
            modelBuilder.Entity<WorldCoral>()
                .Property(w => w.Shape)
                .HasColumnType("geography") // Ensure it maps to a spatial column
                .HasDefaultValueSql("geography::STGeomFromText('POLYGON EMPTY', 4326)");
        }

    }
}

