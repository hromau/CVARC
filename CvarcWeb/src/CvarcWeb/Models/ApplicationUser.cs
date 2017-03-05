using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace CvarcWeb.Models
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        public int CommandId { get; set; }
        public virtual Team Team { get; set; }
        public string FIO { get; set; }
        public string Region { get; set; }
    }
}
