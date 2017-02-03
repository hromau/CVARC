using CVARC.V2;
using System;


namespace Assets
{
    [Obsolete]
    public interface IRunner : IDisposable
    {
        void InitializeWorld();
        IWorld World { get; }
        string Name { get; }
        bool CanStart { get; }
        bool CanInterrupt { get; }
        bool Disposed { get; }
    }
}
