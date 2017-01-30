using System;

namespace CVARC.V2
{
    public interface IReflectableTestAction<TSensorData, TCommand> : ITestAction<TSensorData, TCommand>
    {
        void Reflect(Func<TCommand, TCommand> reflectCommand, Func<TSensorData, TSensorData> reflectSensors);
    }
}
