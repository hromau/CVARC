using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CvarcWeb.Models
{
    public class Result
    {
        public int ResultId { get; set; }
        public virtual TeamGameResult TeamGameResult { get; set; }
        public int Scores { get; set; }
        public string ScoresType { get; set; }
    }
}
