using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CvarcWeb.Data;

namespace CvarcWeb.Models
{
    public class TeamRequest
    {
        public int TeamRequestId { get; set; }
        public virtual Team Team { get; set; }
        public virtual ApplicationUser User { get; set; }
    }
}
