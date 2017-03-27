using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace CvarcWeb.Models.ManageViewModels
{
    public class IndexViewModel
    {
        public bool HasPassword { get; set; }

        public IList<UserLoginInfo> Logins { get; set; }

        public string PhoneNumber { get; set; }

        public bool TwoFactor { get; set; }

        public bool BrowserRemembered { get; set; }

        public IEnumerable<TeamRequest> RequestsInUserTeam { get; set; } 
        public IEnumerable<TeamRequest> UserRequestsInOtherTeam { get; set; } 
        public bool HasTeam { get; set; }
        public Team Team { get; set; }
        public bool HasOwnTeam { get; set; }
        public int MaxSize { get; set; }
        public bool CanOwnerLeave { get; set; }
        public bool HasSolution { get; set; }
    }
}
