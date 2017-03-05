using CvarcWeb.Models;

namespace CvarcWeb.Tournaments.Common
{
    public interface ITournamentViewModel
    {
        string Name { get; set; }
        int Id { get; set; }
        TournamentType Type { get; set; }
    }
}
