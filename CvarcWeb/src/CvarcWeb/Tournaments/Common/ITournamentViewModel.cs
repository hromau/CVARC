using CvarcWeb.Models;

namespace CvarcWeb.Tournaments.Common
{
    public interface ITournamentViewModel
    {
        TournamentType Type { get; set; }
    }
}
