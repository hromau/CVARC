namespace CVARC.V2
{
    public interface ISensorAsserter<in TSensorData>
    {
        void Assert(TSensorData data, IAsserter asserter);
    }
}
