using System.Collections.Generic;

namespace CvarcWeb.Models
{
    public class Game
    {
        public int GameId { get; set; }
        public string GameName { get; set; }
        public string PathToLog { get; set; }
        public virtual ICollection<TeamGameResult> TeamGameResults { get; set; }
    }
}
