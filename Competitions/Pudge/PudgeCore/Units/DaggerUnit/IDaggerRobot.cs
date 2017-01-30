using CVARC.V2;

namespace Pudge.Units.DaggerUnit
{
    public interface IDaggerRobot : IActor
    {
        void Dagger(DaggerDestinationPoint destination);
    }
}