using System;

namespace CvarcWeb.Models
{
    public class TeamRequest
    {
        public int TeamRequestId { get; set; }
        public virtual Team Team { get; set; }
        public virtual ApplicationUser User { get; set; }
    }
}
