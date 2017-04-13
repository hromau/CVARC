using System;
using System.Collections.Generic;

namespace CvarcWeb.Models
{
    public class Team
    {
        public int TeamId { get; set; }
        public string Name { get; set; }
        public string OwnerId { get; set; }
        public Guid CvarcTag { get; set; }
        public string LinkToImage { get; set; }
        public virtual ICollection<ApplicationUser> Members { get; set; }
        public int MaxSize { get; set; }
        public bool CanOwnerLeave { get; set; }
    }
}
