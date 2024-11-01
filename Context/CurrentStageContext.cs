using Microsoft.EntityFrameworkCore;
using Database.Models;

namespace Database.Context
{
    public class CurrentStageContext : DbContext
    {

        public CurrentStageContext(DbContextOptions<CurrentStageContext> options)
            : base(options)
        {
        }

        public DbSet<CurrentStagePOST> CurrentStage { get; set; }
        
    }
}
