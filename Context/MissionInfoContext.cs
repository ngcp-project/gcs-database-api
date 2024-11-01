using Microsoft.EntityFrameworkCore;
using Database.Models;

namespace Database.Context
{
    public class MissionInfoContext : DbContext
    {
        public MissionInfoContext(DbContextOptions<MissionInfoContext> options)
            : base(options)
        {
        }

        public DbSet<MissionInfo> MissionInfo { get; set; }
        public DbSet<MissionInfoGET> MissionInfoGET { get; set; }
        public DbSet<MissionInfoPOST> MissionInfoPOST { get; set; }
    }
}
