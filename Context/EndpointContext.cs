using Microsoft.EntityFrameworkCore;
using Database.Models;

namespace Database.Context
{
    public class EndpointContext : DbContext
    {

        public EndpointContext(DbContextOptions<CurrentStageContext> options)
            : base(options)
        {
        }

    public DbSet<EndpointResponse> EnpointResponses { get; set; }
    public DbSet<EndpointResponse> EnpointReturns { get; set; }
        
    }
}
