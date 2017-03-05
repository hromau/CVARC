using CvarcWeb.Models;
using CvarcWeb.Tournaments.Common;

namespace CvarcWeb.Tournaments.Playoff
{
    public class OlympicTournamentViewModel : ITournamentViewModel
    {
        public OlympicTournamentMatchViewModel FinalMatch { get; set; }
        public OlympicTournamentMatchViewModel ThirdPlaceMatch { get; set; }
        public string Name { get; set; }
        public int Id { get; set; }
        public TournamentType Type { get; set; }

    }
}
