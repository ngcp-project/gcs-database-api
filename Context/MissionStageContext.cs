using Microsoft.EntityFrameworkCore;
using Database.Models;

namespace Database.Context
{
    public class MissionStageContext : DbContext
    {
        public MissionStageContext(DbContextOptions<MissionStageContext> options)
            : base(options)
        {
        }

        public DbSet<MissionStage> MissionStage { get; set; }
        public DbSet<MissionStageGET> MissionStageGET { get; set; }
        public DbSet<MissionStagePOST> MissionStagePOST { get; set; }
        public DbSet<MissionStageQuery> MissionStageQuery { get; set; }
    }
}
