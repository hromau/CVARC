namespace CVARC.V2
{
    public interface INetworkData
    {
        LoadingData LoadingData { get; set; }
        int Port { get; set; }
        IWorld WaitForWorld();
    }
}
