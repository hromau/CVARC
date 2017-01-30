using System;
using System.Linq;

namespace CVARC.V2
{
    public abstract class TestLoadableLogicPartHelper : LogicPartHelper 
    {
        public sealed override LogicPart Create()
        {
            LogicPart logic = new LogicPart();

            LoadTests(logic); 

            return Initialize(logic);
        }

        public abstract LogicPart Initialize(LogicPart logic);

        private void AddTest(LogicPart logic, string name, ICvarcTest test)
        {
            logic.Tests[name] = test;
        }

        private void LoadTests(LogicPart logic)
        {
            GetType().Assembly.GetTypes()
                .Where(type => IsGenericSubclass(type, typeof(CvarcTestCase<,,,,>)))
                .Select(type => Activator.CreateInstance(type) as ICvarcTestCase)
                .Where(instance => instance != null)
                .SelectMany(instance => instance.GetDefinedTests()).ToList()
                .ForEach(test => AddTest(logic, test.Item1, test.Item2));
        }

        private bool IsGenericSubclass(Type derived, Type parent)
        {
            while (derived != null && derived != typeof(object))
            {
                derived = derived.IsGenericType ? derived.GetGenericTypeDefinition() : derived;
                if (derived == parent)
                    return true;

                derived = derived.BaseType;
            }

            return false;
        }
    }
}
