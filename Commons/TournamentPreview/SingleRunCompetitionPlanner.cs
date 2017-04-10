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
        public void Plan(string competitions, GameSettings settings , params object[] worldStates)
        {
            var participants = Json.Read<List<TournamentParticipant>>("participants.json");
            var list = new List<TournamentTask>();
            foreach(var e in participants)
                foreach(var state in worldStates)
                {
                    var task = new TournamentTask
                    {
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
