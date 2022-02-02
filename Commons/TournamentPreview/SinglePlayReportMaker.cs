
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Infrastructure;

namespace TournamentPreview
{

    class UserReport
    {
        public string Name;
        public string Team;
        public int TeamId;
        public ExtractionStatus? Extraction;
        public TournamentVerificationStatus? Verification;
        public int[] Results;
        public double Average;

    }

    class SinglePlayReportMaker
    {
        public static void MakeReport()
        {
            var teams = File.ReadLines("teams.csv")
                .Skip(1)
                .Select(z => z.Split(';'))
                .Select(z => new { name = z[0], id = int.Parse(z[1]), team = z[2] })
                .ToList();

            var extract = Json.Read<List<TournamentParticipantExtraction>>("extract.json")
                .ToDictionary(z => z.Participant.Id, z => z);
            var verify = Json.Read<List<TournamentVerificationResult>>("verify.json")
                .ToDictionary(z => z.Participant.Id, z => z);

            var results = File.ReadLines("results.json").Select(Serializer.Deserialize<TournamentGameResult>)
                .GroupBy(z => z.Task.Participants[0].Id)
                .ToDictionary(z => z.Key, z => z);

            var report = new List<UserReport>();
            
            foreach(var e in teams)
            {
                var r = new UserReport();
                r.Name = e.name;
                r.Team = e.team;
                r.TeamId = e.id;

                if (extract.ContainsKey(e.id))
                    r.Extraction = extract[e.id].Status;
                if (verify.ContainsKey(e.id))
                    r.Verification = verify[e.id].Status;
                if (results.ContainsKey(e.id))
                {
                    r.Results = results[e.id]
                        .Select(z => z.Result.ScoresByPlayer["Left"].Where(x => x.Key == "Main").Select(x=>x.Value).First())
                        .ToArray();
                    r.Average = r.Results.Average();
                }
                report.Add(r);
            }

            using (var wr = new StreamWriter("report.html"))
            {
                wr.WriteLine("<!DOCTYPE html>\n<meta charset=\"UTF-8\">\n<html><body><table>");
                wr.WriteLine("<tr><th>ФИО</th><th>Команда</th><th>ИД команды</th><th>Формат</th><th>Прозвон</th><th>Игра 1</th><th>Игра 2</th><th>Игра 3</th><th>Среднее</th><tr>");
                foreach(var e in report.OrderByDescending(z=>z.Average).ThenBy(z=>!z.Extraction.HasValue).ThenBy(z=>z.TeamId))
                {
                    wr.WriteLine($"<tr><td>{e.Name}</td><td>{e.Team}</td><td>{e.TeamId}</td><td>{e.Extraction}</td><td>{e.Verification}</td>");
                    for (int i = 0; i < 3; i++)
                    {
                        wr.WriteLine("<td>");
                        if (e.Results != null && e.Results.Length > i)
                            wr.WriteLine(e.Results[i]);
                        wr.WriteLine("</td>");
                    }
                    wr.WriteLine($"<td>{e.Average.ToString("0.00")}</td></tr>");
                }
                wr.WriteLine("</table></body></html>");
            }
            Process.Start("report.html");

        }
    }
}
