using System;
using System.Linq;

namespace CVARC.V2
{
    public abstract class TestLoadableLogicPartHelper : LogicPartHelper
    {
        public sealed override LogicPart Create()
        {
            LogicPart logic = new LogicPart();

            //LoadTests(logic); 

            return Initialize(logic);
        }

        public abstract LogicPart Initialize(LogicPart logic);
    }
}
