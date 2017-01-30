using System;

namespace CVARC.V2
{
    class DelegatedAsserter<TSensorData> : ISensorAsserter<TSensorData>
    {
        private readonly Action<TSensorData, IAsserter> _assert;

        public DelegatedAsserter(Action<TSensorData, IAsserter> assert)
        {
            _assert = assert;
        }

        public void Assert(TSensorData data, IAsserter asserter)
        {
            _assert(data, asserter);
        }
    }
}
