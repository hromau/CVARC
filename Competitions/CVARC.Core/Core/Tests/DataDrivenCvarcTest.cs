namespace CVARC.V2
{
    public class DataDrivenCvarcTest<TSensorData, TCommand, TWorld, TWorldState>
        : CvarcTest<TSensorData, TCommand, TWorld, TWorldState>
        where TSensorData : class, new()
        where TWorldState : IWorldState
        where TCommand : ICommand
        where TWorld : IWorld
    {
        public TestData<TSensorData, TCommand, TWorldState> Data { get; private set; }

        public DataDrivenCvarcTest(TestData<TSensorData, TCommand, TWorldState> data)
        {
            Data = data;
        }

        public override SettingsProposal GetSettings() { return Data.Settings; }
        public override TWorldState GetWorldState() { return Data.WorldState; }

        public override void Test(CvarcClient<TSensorData, TCommand> client, TWorld world, IAsserter asserter)
        {
            var sensorData = new TSensorData();

            foreach (var action in Data.Actions)
            {
                if (action.Command != null)
                    sensorData = client.Act(action.Command);
                if (action.Asserter != null)
                    action.Asserter.Assert(sensorData, asserter);
            }
        }
    }
}
