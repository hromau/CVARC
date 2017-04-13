using System.Collections.Generic;
using CvarcWeb.Models;

namespace CvarcWeb.Data.Repositories
{
    public class TournamentsRepository
    {
        private readonly UserDbContext context;
        public TournamentsRepository(UserDbContext context)
        {
            this.context = context;
        }
    }
}
