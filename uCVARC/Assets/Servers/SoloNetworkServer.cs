using CVARC.V2;


namespace Assets
{
    class SoloNetworkServer : UnityServer
    {
        public SoloNetworkServer(int port) : base(port) { }

        protected override void HandleClient(CvarcClient client)
        {
            Dispatcher.AddRunner(new SoloNetworkRunner(client));
        }

        protected override void Print(string str)
        {
            Debugger.Log("network server: " + str);
        }
    }
}
