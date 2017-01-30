using System.Collections.Generic;
using CVARC.V2;
using Pudge.Player;
using Pudge.World;

namespace Pudge.Tests
{
    public class DefaultPudgeTest :
        CvarcTestCase<PudgeRules, PudgeSensorsData, PudgeCommand, PudgeWorldState, PudgeWorld>
    {
        public DefaultPudgeTest()
            : base(PudgeRules.Current, new PudgeWorldState(42), new SettingsProposal())
        {
            DefaultSettings.OperationalTimeLimit = 5;
            DefaultSettings.TimeLimit = 90;
            DefaultSettings.Controllers = new List<ControllerSettings>
            {
                new ControllerSettings {ControllerId = TwoPlayersId.Left, Name = "This", Type = ControllerType.Client},
                new ControllerSettings {ControllerId = TwoPlayersId.Right, Name = PudgeRules.StandingBotName,
                    Type = ControllerType.Bot}
            };
        }
    }
}