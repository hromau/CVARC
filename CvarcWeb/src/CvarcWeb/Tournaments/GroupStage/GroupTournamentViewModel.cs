using CvarcWeb.Models;
using CvarcWeb.Tournaments.Common;

namespace CvarcWeb.Tournaments.GroupStage
{
    public class GroupTournamentViewModel : ITournamentViewModel
    {
        public TournamentType Type { get; set; }
        public GroupViewModel[] Groups { get; set; }
    }

    public class GroupViewModel
    {
        public string GroupName { get; set; }
        public Game[][] Games { get; set; }
    }
}
