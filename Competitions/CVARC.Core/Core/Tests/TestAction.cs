namespace CVARC.V2
{
    public class TestAction<TSensorData, TCommand> : ITestAction<TSensorData, TCommand>
        where TCommand : ICommand
    {
        public TCommand Command { get; protected set; }
        public ISensorAsserter<TSensorData> Asserter { get; protected set; }

        public TestAction(TCommand command)
        {
            Command = command;
        }

        public TestAction(ISensorAsserter<TSensorData> asserter)
        {
            Asserter = asserter;
        }
    }
}

