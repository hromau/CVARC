using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CvarcWeb.Models
{
    public class TeamLog
    {
        public int Id { get; set; }
        public Team Team { get; set; }
        public ApplicationUser User { get; set; }
        public int Action { get; set; } //0 - Create, 1 - Join, 2 - Leave
        public DateTime Time { get; set; }
    }
}
