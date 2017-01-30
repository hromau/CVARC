using System.Collections.Generic;
using System.Linq;

namespace CVARC.V2
{
    public class TestData<TSensorData, TCommand, TWorldState>
        where TWorldState : IWorldState
        where TCommand : ICommand
    {
        public TWorldState WorldState { get; private set; }
        public SettingsProposal Settings { get; private set; }
        public IEnumerable<ITestAction<TSensorData, TCommand>> Actions { get; private set; }

        public TestData(TWorldState worldState, SettingsProposal settings, 
            IEnumerable<ITestAction<TSensorData, TCommand>> actions)
        {
            WorldState = worldState;
            Settings = settings;
            Actions = actions.ToList();
        }
    }
}
