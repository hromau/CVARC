namespace CvarcWeb.Models
{
    public class Tournament
    {
        public int TournamentId { get; set; }
        public string Name { get; set; }
        public string TournamentTree { get; set; }
        public TournamentType Type { get; set; }
    }
}
