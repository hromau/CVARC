using Infrastructure;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TournamentPreview
{
    public static class E
    {

        public static T SafeGet<T1, T>(this Dictionary<T1, T> d, T1 k)
            where T: new()
        {
            if (!d.ContainsKey(k)) d[k] = new T();
            return d[k];
        }
    }

    class GroupCompetitionPlanner
    {
        List<List<TournamentParticipant>> GetGroups(int seed, int[] counts)
        {
            var participants = Json.Read<List<TournamentParticipant>>("participants.json");
            if (participants.Count != counts.Sum())
                throw new Exception();
            Random rnd = new Random(seed);
            var perm = Enumerable.Range(0, participants.Count).ToArray();
            for (int i = 0; i < 1000; i++)
            {
                var e = rnd.Next(perm.Length - 1);
                var t = perm[e];
                perm[e] = perm[e + 1];
                perm[e + 1] = t;
            }
            var groups = new List<List<TournamentParticipant>>();
            int pCounter = 0;
            for (int i = 0; i < counts.Length; i++)
            {
                groups.Add(new List<TournamentParticipant>());
                for (int j = 0; j < counts[i]; j++)
                {
                    groups[i].Add(participants[perm[pCounter]]);
                    pCounter++;
                }
            }
            return groups;
        }


        public static void Show(TextWriter wr=null)
        {
            if (wr == null)
                wr = Console.Out;
            var gr = Json.Read<List<TournamentTask[,]>>("groups.json");
            for (int i = 0; i < gr.Count; i++)
            {
                wr.WriteLine($"Group {i}");
                var g = gr[i];
                for (int row = 0; row < g.GetLength(0); row++)
                {
                    for (int column = 0; column < g.GetLength(1); column++)
                    {
                        var str = g[row, column]?.Id ?? "";
                        wr.Write("{0,10}", str);
                    }
                    wr.WriteLine();
                }
            }
        }

        static int GetScores(TournamentGameResult res, string side)
        {
            return res.Result.ScoresByPlayer[side]["Main"];
        }



        public static void ShowGroupedResults(TextWriter wr=null)
        {
            if (wr == null)
                wr = Console.Out;

            var r = new Dictionary<int, List<int>>();

            var teams = File.ReadLines("teams.csv")
                .Select(z => z.Split(';'))
                .ToDictionary(z => int.Parse(z[0]), z => z[1]);
            var results = File.ReadLines("results.json").Select(JsonConvert.DeserializeObject<TournamentGameResult>).ToList();
            foreach(var e in results)
            {
                r.SafeGet(e.Task.Participants[0].Id).Add(GetScores(e, "Left"));
                r.SafeGet(e.Task.Participants[1].Id).Add(GetScores(e, "Right"));
            }

            var s = r.Select(x => new { x.Key, Av = x.Value.Average(), Cn = x.Value.Count() })
                .OrderByDescending(z => z.Av)
                .Select((z,i) => string.Format("{0,-3}{1,-40}{2,-5}{3,-3} {4}",i, teams[z.Key], z.Key, z.Cn, z.Av))
                .Aggregate((a, b) => a + "\n" + b);

            wr.WriteLine(s);


        }

        public static void ShowResults(TextWriter wr=null)
        {
            if (wr == null)
                wr = Console.Out;
            var gr = Json.Read<List<TournamentTask[,]>>("groups.json");
            var results = File.ReadLines("results.json").Select(JsonConvert.DeserializeObject<TournamentGameResult>).ToList();

            for (int i = 0; i < gr.Count; i++)
            {
                wr.WriteLine($"Group {i}");
                var g = gr[i];
                
                for (int row = 0; row < g.GetLength(0); row++)
                {
                    for (int column = 0; column < g.GetLength(1); column++)
                    {
                        if (g[row, column] == null)
                        {
                            wr.Write("{0,19}", "");
                            continue;
                        }
                        var id1 = g[row, column].Participants[0].Id;
                        var id2 = g[row, column].Participants[1].Id;
                        var rs = results.Where(z => z.Task.Id == g[row, column].Id).SingleOrDefault();
                        if (rs == null)
                        {
                            wr.Write("{0,19}", "");
                            continue;
                        }
                        var sc1 = GetScores(rs, "Left");
                        var sc2 = GetScores(rs, "Right");
                        wr.Write("{0,3}:{1,3} - {2,3}:{3,3}  ", id1, sc1, id2, sc2);
                    }
                    wr.WriteLine();
                }
            }
        }

        

        public void Plan(GameSettings settings, int[] counts, object[] worldStates, int seed)
        {
            var groups = GetGroups(seed, counts);
            var list = new List<TournamentTask>();
            var gr = new List<TournamentTask[,]>();
            int stateCounter = 0;
            foreach (var e in groups)
            {
                var currentGr = new TournamentTask[e.Count, e.Count];
                gr.Add(currentGr);
                
                for (int first = 0; first < e.Count - 1; first++)
                    for (int second = first + 1; second < e.Count; second++)
                    {
                        var p1 = e[first];
                        var p2 = e[second];
                        var state = worldStates[stateCounter];
                        var task = new TournamentTask
                        {
                            Id = stateCounter + "-" + p1.Id + "-" + p2.Id,
                            GameSettings = settings,
                            WorldState = JObject.FromObject(state),
                            Participants = new List<TournamentParticipant>
                               {
                                   p1,
                                   p2
                               }
                        };
                        currentGr[first, second] = task;
                        list.Add(task);
                        stateCounter = (stateCounter + 1) % worldStates.Length;
                    }
            }
            Json.Write("tournament.json", list);
            Json.Write("groups.json", gr);
        }
    }
}