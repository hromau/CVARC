using CvarcWeb.Models;

namespace CvarcWeb.Tournaments.Playoff

{
    public class OlympicTournamentMatchViewModel
    {
        public OlympicTournamentMatchViewModel(Game game)
        {
            Game = game;
        }
        public Game Game { get; set; }
        public OlympicTournamentMatchViewModel FirstPreviousStageMatch { get; set; }
        public OlympicTournamentMatchViewModel SecondPreviousStageMatch { get; set; }
    }
}
