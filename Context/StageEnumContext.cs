using Microsoft.EntityFrameworkCore;

namespace Database.Context
{
    public class StageEnumContext : DbContext
    {
        public StageEnumContext(DbContextOptions<StageEnumContext> options) : base(options) { }
        public DbSet<StageEnumWrapper> StageEnum { get; set; }
    }

    public class StageEnumWrapper
    {
        public Stage_Enum Stage { get; set; }
    }
}
