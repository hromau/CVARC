using CvarcWeb.Models;
using Microsoft.EntityFrameworkCore;

namespace CvarcWeb.Data
{
    public class CvarcDbContext : DbContext
    {
        public CvarcDbContext(DbContextOptions<CvarcDbContext> options)
            : base(options)
        {
        }

        public DbSet<Team> Teams { get; set; }
        public DbSet<Game> Games { get; set; }
        public DbSet<Result> Results { get; set; }
        public DbSet<TeamGameResult> TeamGameResults { get; set; }
        public DbSet<Tournament> Tournaments { get; set; }
    }
}
