using Infrastructure;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Infrastructure
{
    public class TournamentParticipant
    {
        public string PathToExe { get; set; }
        public int Id { get; set; }
    }

    public enum TournamentVerificationStatus
    {
        OK,
        FileNotFound,
        UnableToRun,
        ConnectionFailed,
        UnknownError,
        WrongFormat,
        SecondConnectionDetected
    }

    public class TournamentVerificationResult
    {
        public TournamentParticipant Participant { get; set; }
        public TournamentVerificationStatus Status { get; set; }
        public string AdditinalInformation { get; set; }
    }

    public class TournamentTask
    {
        public List<TournamentParticipant> Participants { get; set; } = new List<TournamentParticipant>();
        public GameSettings GameSettings { get; set; }
        public JObject WorldState { get; set; }
        public string Id { get; set; }
    }

    public class TournamentGameResult
    {
        public TournamentTask Task { get; set; }
        public GameResult Result { get; set; }
    }

}
