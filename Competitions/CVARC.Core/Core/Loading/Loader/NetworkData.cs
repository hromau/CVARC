using System;

namespace CVARC.V2
{
    public class NetworkData : INetworkData
    {
        public LoadingData LoadingData { get; set; }
        public int Port { get; set; }
        public IWorld World { get; set; }
        public string Name { get; set; }
        public Action<NetworkData> WaitWorld { get; set; }

        public IWorld WaitForWorld()
        {
            if (WaitWorld != null)
                WaitWorld(this);
            if (World == null)
                throw new Exception("Wait world failed");
            return World;
        }
    }
}
