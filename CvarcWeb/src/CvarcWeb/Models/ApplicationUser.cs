using System;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace CvarcWeb.Models
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        public virtual Team Team { get; set; }
        public string FIO { get; set; }
        public string Region { get; set; }
    }
}
