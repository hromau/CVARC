using CVARC.V2;
using System;
using System.Linq;
using System.Windows.Forms;
using AIRLab.Mathematics;

namespace Pudge.World
{
    class RuneRespawnTrigger : Trigger // -> TimerTrigger
    {
        private PudgeWorld world;
        private Random random;

        public double Interval { get; private set; }
        
        public RuneRespawnTrigger(PudgeWorld world, double interval=double.PositiveInfinity)
        {
            this.world = world;
            random = new Random(world.WorldState.Seed);
            Interval = interval;
            ScheduledTime = -1;
        }

        public override TriggerKeep Act(double time)
        {
            RespawnRunes();
            ScheduledTime += Interval;
            return TriggerKeep.Keep;
        }

        protected virtual void RespawnRunes()
        {
            RemoveAllRunesOnField();


            var firstPudgeRune = RandomRuneType();
            SpawnRune(100, 0, firstPudgeRune);
            SpawnRune(-100, 0, firstPudgeRune);


            var secondPudgeRune = RandomRuneType();
            SpawnRune(0, 100, secondPudgeRune);
            SpawnRune(0, -100, secondPudgeRune);

            SpawnRune(0, 0, RandomRuneType(), RuneSize.Large);
            SpawnRune(130, -130, RandomRuneType(), RuneSize.Large);
            SpawnRune(-130, 130, RandomRuneType(), RuneSize.Large);


        }
        private RuneType RandomRuneType()
        {
            return (RuneType) random.Next(0, Enum.GetNames(typeof (RuneType)).Length);
        }


        protected void SpawnRune(double X, double Y, RuneType type, RuneSize size = RuneSize.Normal)
        {
            var rune = new InternalRuneData(type, size, new Frame3D(X,Y,0));
            var id = world.IdGenerator.CreateNewId(rune);

            var p = rune.Location;
            world.GetEngine<IPudgeWorldEngine>().SpawnRune(rune.Type, rune.Size, p.X, p.Y, p.Z, id);
            world.SpawnedRunes.Add(rune);


        }

        protected void RemoveAllRunesOnField()
        {
            var engine = world.GetEngine<ICommonEngine>();
            var runesOnField = world.IdGenerator
                .GetAllPairsOfType<InternalRuneData>()
                .Where(pair => engine.ContainBody(pair.Item2))
                .Select(pair => pair.Item2);
            
            foreach (var runeId in runesOnField)
                engine.DeleteObject(runeId);

            world.SpawnedRunes.Clear();
        }
    }
}
