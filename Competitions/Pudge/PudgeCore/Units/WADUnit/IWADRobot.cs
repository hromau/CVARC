using CVARC.V2;

namespace Pudge.Units.WADUnit
{
    public interface IWADRobot : IActor
    {
        double HasteFactor{ get; }
        void Die();
    }
}
