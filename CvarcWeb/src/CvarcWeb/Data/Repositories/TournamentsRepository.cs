using System.Collections.Generic;
using CvarcWeb.Models;

namespace CvarcWeb.Data.Repositories
{
    public class TournamentsRepository
    {
        private readonly CvarcDbContext context;
        public TournamentsRepository(CvarcDbContext context)
        {
            this.context = context;
        }
    }
}
