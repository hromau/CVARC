namespace CvarcWeb.Models
{
    public class OlympicTournamentMatch
    {
        public OlympicTournamentMatch(int gameId)
        {
            GameId = gameId;
        }
        public OlympicTournamentMatch FirstPreviousStageMatch { get; set; }
        public OlympicTournamentMatch SecondPreviousStageMatch { get; set; }
        public int GameId { get; set; }
    }
}
