using System;
using HoMM.ClientClasses;
using HoMM.Robot;
using HoMM.Robot.ScoutInterface;

namespace HoMM
{
    public sealed class HommLevel1Client : BaseHommClient<HommSensorData> {
        protected override HommLevel Level => HommLevel.Level1;
    }

    public sealed class HommLevel2Client : BaseHommClient<HommSensorData>
    {
        protected override HommLevel Level => HommLevel.Level2;
    }

    public sealed class HommLevel3Client : BaseHommClient<HommSensorData>
    {
        protected override HommLevel Level => HommLevel.Level3;
    }

    public sealed class HommFinalLevelClient : BaseHommClient<HommFinalSensorData>
    {
        protected override HommLevel Level => HommLevel.Final;

        public HommFinalSensorData ScoutTile(LocationInfo locationToScan)
        {
            return Act(new HommCommand { ScoutOrder = ScoutOrder.ScoutTileOrder(locationToScan) });
        }

        public HommFinalSensorData ScoutHero()
        {
            return Act(new HommCommand { ScoutOrder = ScoutOrder.ScoutHeroOrder() });
        }

    }

    public sealed class HommClient : BaseHommClient<HommSensorData>
    {
        protected override HommLevel Level => throw new NotImplementedException();

        [Obsolete("В новых версиях следует вместо `HommClient` использовать клиентов типов `HommLevelXClient`")]
        public new HommSensorData Configurate(string ip, int port, Guid cvarcTag,
            HommLevel level = HommLevel.Level1,
            bool isOnLeftSide = true,
            int timeLimit = 90,
            int operationalTimeLimit = 1000,
            int seed = 0,
            bool speedUp = false,
            bool debugMap = false,
            bool spectacularView = true)
        {
            if (level == HommLevel.Final) throw new ArgumentException("Should use HommFinalLevelClient to launch final level.");

            return base.Configurate(ip, port, cvarcTag, level, isOnLeftSide, timeLimit, 
                operationalTimeLimit, seed, speedUp, debugMap, spectacularView);
        }
    }
}
