using CvarcWeb.Models;
using CvarcWeb.Tournaments.Common;

namespace CvarcWeb.Tournaments.GroupStage
{
    public class GroupTournamentViewModel : ITournamentViewModel
    {
        public string GroupName { get; set; }
        public Game[][] Games { get; set; }
        public TournamentType Type { get; set; }
        public string Name { get; set; }
        public int Id { get; set; }

    }
}
