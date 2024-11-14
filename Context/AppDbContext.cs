using Microsoft.EntityFrameworkCore;
using Database.Models;
using Database.Controllers;
using Microsoft.AspNetCore.Mvc.ModelBinding;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    
    // These are the tables that are being created (idk how the table structure is)
    public DbSet<MissionInfo> MissionInfos { get; set; }
    // public DbSet<MissionStage> MissionStages { get; set; }
    // public DbSet<Geofence> Geofences { get; set; }

    // might need OnModelCreating or something to define relationships between tables
}