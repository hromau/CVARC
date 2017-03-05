using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Infrastructure
{
    // При изменении этой модели нужно обязательно поменять модель в веб солюшне.
    public class WebCommonResults
    {
        public string GameName { get; set; }
        public string PathToLog { get; set; }
        public Dictionary<string, Guid> RoleToCvarcTag { get; set; }
        public Dictionary<string, Dictionary<string, int>> Scores { get; set; }
    }
}
