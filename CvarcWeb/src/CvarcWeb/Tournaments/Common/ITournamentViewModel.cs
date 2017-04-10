using CvarcWeb.Models;
using CvarcWeb.Tournaments.GroupStage;

namespace CvarcWeb.Tournaments.Common
{
    public interface ITournamentViewModel
    {
        GroupViewModel[] Groups { get; set; }
        TournamentType Type { get; set; }
    }
}
