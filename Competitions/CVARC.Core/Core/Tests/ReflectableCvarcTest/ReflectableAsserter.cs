using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CVARC.V2
{
    class ReflectableAsserter<TSensorData> : ISensorAsserter<TSensorData>
    {
        private ISensorAsserter<TSensorData> wrappedAsserter;
        private Func<TSensorData, TSensorData> sensorsReflector;

        public ReflectableAsserter(ISensorAsserter<TSensorData> wrappedAsserter, 
            Func<TSensorData, TSensorData> sensorsReflector)
        {
            this.wrappedAsserter = wrappedAsserter;
            this.sensorsReflector = sensorsReflector;
        }
        
        public void Assert(TSensorData data, IAsserter asserter)
        {
            wrappedAsserter.Assert(sensorsReflector(data), asserter);
        }
    }
}
