using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    // Add DbSet<T> properties here once know table structures
    public DbSet<MissionInfo> MissionInfos { get; set; }
    public DbSet<MissionStage> MissionStages { get; set; }
    public DbSet<Geofence> Geofences { get; set; }

}
