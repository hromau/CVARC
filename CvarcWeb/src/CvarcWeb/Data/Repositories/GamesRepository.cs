using System.Collections.Generic;
using System.Linq;
using CvarcWeb.Models;
using Microsoft.EntityFrameworkCore;

namespace CvarcWeb.Data.Repositories
{
    public class GamesRepository
    {
        private readonly CvarcDbContext context;
        public GamesRepository(CvarcDbContext context)
        {
            this.context = context;
        }

        public IEnumerable<Game> GetByIds(IEnumerable<int> gameIds)
        {
            var ids = new HashSet<int>(gameIds);
            return context.Games
                          .Include(g => g.TeamGameResults).ThenInclude(cgr => cgr.Team)
                          .Include(g => g.TeamGameResults).ThenInclude(cgr => cgr.Results)
                          .Where(g => ids.Contains(g.GameId));
        }
    }
}
