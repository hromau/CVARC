using System.Collections.Generic;

namespace CvarcWeb.Models
{
    public class TeamGameResult
    {
        public int TeamGameResultId { get; set; }
        public virtual Game Game { get; set; }
        public virtual Team Team { get; set; }
        public virtual ICollection<Result> Results { get; set; }
    }
}
