using System;
using System.Collections.Generic;

namespace CVARC.V2
{   
    class TestBuilder<TSensorData, TCommand, TWorldState, TWorld>
        where TSensorData : class, new()
        where TCommand : class, ICommand
        where TWorldState : IWorldState
        where TWorld : IWorld
    {
        protected List<ITestAction<TSensorData, TCommand>> CurrentTest { get; private set; }

        public TestBuilder()
        {
            CurrentTest = new List<ITestAction<TSensorData, TCommand>>();
        }

        public virtual CvarcTest<TSensorData, TCommand, TWorld, TWorldState> CreateTest(TWorldState worldState, SettingsProposal settings)
        {
            var data = new TestData<TSensorData, TCommand, TWorldState>(worldState, SettingsProposal.DeepCopy(settings), CurrentTest);
            CurrentTest = new List<ITestAction<TSensorData, TCommand>>();
            return new DataDrivenCvarcTest<TSensorData, TCommand, TWorld, TWorldState>(data);
        }

        public virtual void AddCommand(TCommand command)
        {
            CurrentTest.Add(new TestAction<TSensorData, TCommand>(command));
        }

        public virtual void AddAssert(ISensorAsserter<TSensorData> assert)
        {
            CurrentTest.Add(new TestAction<TSensorData, TCommand>(assert));
        }

        public virtual void AddAssert(Action<TSensorData, IAsserter> assert)
        {
            CurrentTest.Add(new TestAction<TSensorData, TCommand>(new DelegatedAsserter<TSensorData>(assert)));
        }
    }
}
