
namespace Pudge.Units.DaggerUnit
{
    public interface IDaggerCommand
    {
        DaggerDestinationPoint DaggerDestination{ get; set; }
        bool MakeDagger { get; set; }
    }
}