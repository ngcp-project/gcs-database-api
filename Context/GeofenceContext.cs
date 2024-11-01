using Microsoft.EntityFrameworkCore;
using Database.Models;

namespace Database.Context
{
    public class GeofenceContext : DbContext
    {
        public GeofenceContext(DbContextOptions<GeofenceContext> options)
            : base(options)
        {
        }

        public DbSet<Geofence> Geofence { get; set; }
    }
}