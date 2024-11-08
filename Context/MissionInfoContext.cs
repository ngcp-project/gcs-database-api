using Microsoft.EntityFrameworkCore;

namespace database.Context
{
    public class MissionInfoContext : DbContext
    {
        public MissionInfoContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<MissionInfo> MissionInfos { get; set; }
    }
}
