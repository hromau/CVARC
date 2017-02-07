using CVARC.V2;
using HoMM.World;

namespace HoMM.Robot
{
    public interface IHommRobot : IActor
    {
        new HommWorld World { get; }
        Player Player { get; }

        void Die();
    }
}
