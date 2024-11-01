using Microsoft.EntityFrameworkCore;
using Database.Models;

namespace Database.Context
{
    public class StatusEnumContext : DbContext
    {
        public StatusEnumContext(DbContextOptions<StatusEnumContext> options) : base(options) { }
        public DbSet<StatusEnumWrapper> StatusEnum { get; set; }
    }

    public class StatusEnumWrapper
    {
        public Status_Enum Status { get; set; }
    }
}
