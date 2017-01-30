using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AIRLab;

namespace CVARC.V2
{
    public interface ICvarcTestCase
    {
        IEnumerable<Tuple<string, ICvarcTest>> GetDefinedTests();
    }

    public abstract class CvarcTestCase<TRules, TSensorData, TCommand, TWorldState, TWorld> 
        : ICvarcTestCase
        
        where TSensorData : class, new()
        where TWorldState : class, IWorldState
        where TCommand : class, ICommand
        where TWorld : IWorld
        where TRules : IRules

    {
        protected CommandBuilder<TRules, TCommand> Robot { get; private set; }

        private readonly ReflectableTestBuilder<TSensorData, TCommand, TWorldState, TWorld> _testBuilder;
        protected SettingsProposal DefaultSettings { get; private set; }
        protected TWorldState DefaultWorldState { get; private set; }

        protected Func<TCommand, TCommand> CommandReflector
        {
            get { return _testBuilder.CommandReflector; }
            set { _testBuilder.CommandReflector = value; }
        }

        protected Func<TSensorData, TSensorData> SensorsReflector
        {
            get { return _testBuilder.SensorsReflector; }
            set { _testBuilder.SensorsReflector = value; }
        }

        protected CvarcTestCase(TRules rules, TWorldState worldState, SettingsProposal defaultSettings)
        {
            _testBuilder = new ReflectableTestBuilder<TSensorData, TCommand, TWorldState, TWorld>();
            DefaultSettings = defaultSettings;
            DefaultWorldState = worldState;

            Robot = new CommandBuilder<TRules, TCommand>(rules);
            Robot.CommandAdded += c => _testBuilder.AddCommand(c);
        }

        public IEnumerable<Tuple<string, ICvarcTest>> GetDefinedTests()
        {
            return GetType()
                .GetMethods()
                .Where(m => m.GetCustomAttributes(true).Count(a => a is CvarcTestMethod) != 0)
                .Select(BuildTest);
        }

        private Tuple<string, ICvarcTest> BuildTest(MethodInfo testMethod)
        {
            testMethod.Invoke(this, new object[0]);
            var settings = MutateSettings(testMethod, DefaultSettings);
            var worldState = MutateWorldState(testMethod, DefaultWorldState);
            var reflected = IsReflected(testMethod);
            return Tuple.Create(testMethod.Name, (ICvarcTest)_testBuilder.CreateTest(worldState, settings, reflected));
        }

        private static bool IsReflected(MethodInfo method)
        {
            return method
                .GetCustomAttributes(true)
                .Where(a => a is TestSettings.SetReflected)
                .Select(a => (TestSettings.SetReflected)a)
                .Any(a => a.Reflected);
        }

        private static TWorldState MutateWorldState(MethodInfo method, TWorldState defaultWorldState)
        {
            return method
                .GetCustomAttributes(true)
                .Where(a => a is TestSettings.WorldState)
                .Select(a => (TestSettings.WorldState)a)
                .Select(a => (TWorldState)a.State)
                .FirstOrDefault() ?? defaultWorldState;
        }

        private static SettingsProposal MutateSettings(MethodInfo method, SettingsProposal source)
        {
            var newSettings = SettingsProposal.DeepCopy(source);
            return method.GetCustomAttributes(true)
                .Where(attr => attr is SettingsProposalMutator)
                .Select(attr => attr as SettingsProposalMutator)
                .Aggregate(newSettings, (set, mut) => mut.Mutate(set));
        }

        protected void AssertEqual(Func<TSensorData, double> actual, double expected, double delta)
        {
            _testBuilder.AddAssert((s, a) => a.IsEqual(expected, actual(s), delta));
        }

        protected void AssertEqual(Func<TSensorData, int> actual, int expected)
        {
            _testBuilder.AddAssert((s, a) => a.IsEqual(expected, actual(s), 0));
        }

        protected void AssertTrue(Func<TSensorData, bool> condition)
        {
            AssertBool(condition, true);
        }

        protected void AssertFalse(Func<TSensorData, bool> condition)
        {
            AssertBool(condition, false);
        }

        private void AssertBool(Func<TSensorData, bool> condition, bool expected)
        {
            _testBuilder.AddAssert((s, a) => a.IsEqual(expected, condition(s)));
        }
    }
}
