using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CVARC.V2;
using AIRLab.Mathematics;

namespace Pudge
{
    public interface IPudgeWorldEngine : IEngine
    {
        #region PudgeActorManager
        void PlayAnimation(string actorId, Pudge.Animation animation);
        void SetTransparent(string actorId, bool isTransparent);
        void TurnIntoCorpse(string actorId, string corpseId);
        void CreateActorBody(IActor actor, bool debug);
        void CreatePudgeBody(string actorId, string controllerId);
        void CreateSlardarBody(string actorId, string controllerId, bool debug);
        #endregion

        #region PudgeWorldManager
        void CreateEmptyMap();
        void CreateTree(int x, int z, float angle);

        void CreateWard(string objectId, Frame3D location);
        void SpawnHook(double x, double y, double yawGrad, string id);
        void SpawnRune(Pudge.RuneType type, Pudge.RuneSize size, double x, double y, double z, string id);

        void MakeDaggerExplosion(Frame3D location);
        #endregion
    }
}
