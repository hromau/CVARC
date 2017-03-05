using System;
using System.Collections.Generic;

namespace CvarcWeb.Models
{
    public class Team
    {
        public int TeamId { get; set; }
        public string Name { get; set; }
        public ApplicationUser Owner { get; set; }
        public Guid CvarcTag { get; set; }
        public string LinkToImage { get; set; }
        public virtual ICollection<ApplicationUser> Members { get; set; }
    }
}
