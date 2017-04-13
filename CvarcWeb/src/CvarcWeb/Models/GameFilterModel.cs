namespace CvarcWeb.Models
{
    public class GameFilterModel
    {
        public int? GameId { get; set; }
        public string GameName { get; set; }
        public string TeamName { get; set; }
        public string Region { get; set; }
        public int Page { get; set; }
    }
}
