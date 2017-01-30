namespace CVARC.V2
{
    public interface ITestAction<TSensorData, TCommand>
    {
        TCommand Command { get; }
        ISensorAsserter<TSensorData> Asserter { get; }
    }
}
