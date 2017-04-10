using Infrastructure;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TournamentPreview
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length!=1)
            {
                Console.WriteLine("Exacly one argument requried: path to file with list of participants to check");
                return;
            }
            var file = new FileInfo(args[0]);
            var list = JsonConvert.DeserializeObject<List<TournamentParticipant>>(File.ReadAllText(args[0]));
            using (var v = new TournamentVerifier())
            {
                var result = v.Verify(list);
                var fname = Path.Combine(file.Directory.FullName, "verification.json");
                File.WriteAllText(fname,JsonConvert.SerializeObject(result, Formatting.Indented));
            }
        }
    }
}
