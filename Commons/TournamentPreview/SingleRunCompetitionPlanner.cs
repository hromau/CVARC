using Infrastructure;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TournamentPreview
{
    class SingleRunCompetitionPlanner
    {
        public void Plan(GameSettings settings , params object[] worldStates)
        {
            var participants = Json.Read<List<TournamentParticipant>>("participants.json");
            var list = new List<TournamentTask>();
            int counter = 0;
            foreach(var e in participants)
                for (int i = 0; i<worldStates.Length;i++)
                {
                    var state = worldStates[i];
                    var task = new TournamentTask
                    {
                        Id = i.ToString() + "-" + e.Id,
                        Participants = new List<TournamentParticipant> { e },
                        GameSettings = settings,
                        WorldState = JObject.FromObject(state)
                    };
                    list.Add(task);
                }
            Json.Write("tournament.json", list);
        }
    }
}
