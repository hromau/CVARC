using System;
using System.Collections.Generic;
using System.Linq;
using CVARC.V2;
using Pudge.Player;

namespace Pudge.World
{
    public class PudgeWorld : World<PudgeWorldState>
    {
        private Random random;
        public readonly List<InternalRuneData> SpawnedRunes = new List<InternalRuneData>(); 

        public PudgeWorld(bool debugMode = false) : base()
        {
            this.DebugMode = debugMode;
            random = new Random();
        }

        public override void AdditionalInitialization()
        {
            Scores.Add(TwoPlayersId.Left, 0, "Staring scores");
            Scores.Add(TwoPlayersId.Right, 0, "Staring scores");

            Clocks.AddTrigger(new RuneTrigger(this));

            if (DebugMode)
                Clocks.AddTrigger(new DebugRuneRespawnTrigger(this));
            else
                Clocks.AddTrigger(new RuneRespawnTrigger(this, PudgeRules.Current.RuneRespawnTime));

            // logging all IActors now
            LoggingPositionObjectIds.AddRange(Actors.Select(a => a.ObjectId));
            LoggingPositionObjectIds.AddRange(
                IdGenerator.GetAllPairs()
                .Where(t => t.Item1 is Hook)
                .Select(t => t.Item2));
        }

        public override void CreateWorld()
        {
            GetEngine<IPudgeWorldEngine>().CreateEmptyMap();
            CreateForestBorders(32, 32);
            CreateCentralWoods();
            CreateCornerForestRooms();
            Debugger.Log("World initialization finished!");
        }



        private void CreateCentralWoods()
        {
            // Mid (bottom)
            CreateForestLine(0, -6, 7, -11);
            CreateForestLine(6, 0, 11, -7);

            // Mid (top)
            CreateForestLine(0, 6, -7, 11);
            CreateForestLine(-6, 0, -11, 7);

            CreateForestLine(8, 4, 6, 0);
            CreateForestLine(-8, -4, -6, 0);

            CreateForestLine(4, 8, 0, 6);
            CreateForestLine(-4, -8, 0, -6);

            CreateForestLine(15, 0, 12, 0);
            CreateForestLine(-15, 0, -12, 0);

            CreateForestLine(0, 15, 0, 12);
            CreateForestLine(0, -15, 0, -12);
        }

        private void CreateCornerForestRooms()
        {
            CreateForestLine(8, 14, 8, 11);
            CreateForestLine(14, 8, 11, 8);

            CreateForestLine(-8, -14, -8, -11);
            CreateForestLine(-14, -8, -11, -8);
        }

        private void CreateForestBorders(int sizeX, int sizeZ)
        {
            int halfX = sizeX / 2;
            int halfZ = sizeZ / 2;

            CreateForestLine(-halfX, -halfZ, -halfX, halfZ);
            CreateForestLine(-halfX, -halfZ, halfX, -halfZ);

            CreateForestLine(halfX, halfZ, halfX, -halfZ);
            CreateForestLine(halfX, halfZ, -halfX, halfZ);
        }

        private void CreateForestLine(int startX, int startZ, int endX, int endZ)
        {
            if (Math.Abs(startZ - endZ) > Math.Abs(startX - endX))
                CreateBresenhamForest(startZ, startX, endZ, endX, true);
            else
                CreateBresenhamForest(startX, startZ, endX, endZ, false);
        }

        private void CreateBresenhamForest(int startX, int startZ, int endX, int endZ, bool exchange)
        {
            int deltaX = Math.Abs(startX - endX);
            int deltaZ = Math.Abs(startZ - endZ);
            int dx = Math.Sign(endX - startX);
            int dz = Math.Sign(endZ - startZ);
            int error = 0;
            int deltaError = deltaZ;
            int z = startZ;

            for (int x = startX; x != endX; x += dx)
            {
                GetEngine<IPudgeWorldEngine>().
                    CreateTree(exchange ? z : x, exchange ? x : z, (float)random.NextDouble() * 360);
                error += deltaError;
                if (error * 2 >= deltaX)
                {
                    z += dz;
                    error -= deltaX;
                }
            }
        }


    }
}
