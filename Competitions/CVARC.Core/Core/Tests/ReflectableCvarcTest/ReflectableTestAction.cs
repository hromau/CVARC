using System;

namespace CVARC.V2
{
    public class ReflectableTestAction<TSensorData, TCommand> 
        : TestAction<TSensorData, TCommand>, IReflectableTestAction<TSensorData, TCommand>
        where TCommand : ICommand
    {
        private readonly Func<TCommand, TCommand> reflectCommand;
        private readonly Func<TSensorData, TSensorData> reflectSensors;

        public ReflectableTestAction(TCommand command)
            : base(command)
        { }

        public ReflectableTestAction(ISensorAsserter<TSensorData> asserter)
            : base(asserter)
        { }

        public void Reflect(Func<TCommand, TCommand> reflectCommand, Func<TSensorData, TSensorData> reflectSensors)
        {
            if (reflectCommand != null && Command != null)
                Command = reflectCommand(Command);
            if (reflectSensors != null && Asserter != null)
                Asserter = new ReflectableAsserter<TSensorData>(Asserter, reflectSensors);
        }
    }
}
