using AIRLab.Mathematics;
using CVARC.V2;
using Pudge.World;

namespace Pudge.Units.HookUnit
{
    public interface IHookRobot : IActor
    {
        void ActivateBuff(PudgeEvent type, double duration);
        void DeleteBuff(PudgeEvent type);
        void SpawnHook(Frame3D startingLocation, string id);
    }
}