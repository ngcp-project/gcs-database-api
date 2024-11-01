using Microsoft.EntityFrameworkCore;
using Database.Models;

namespace Database.Context
{
    public class CoordinateContext : DbContext
    {

        public CoordinateContext(DbContextOptions<CoordinateContext> options)
            : base(options)
        {
        }

        public DbSet<Coordinate> Coordinate { get; set; }
        
    }
}
