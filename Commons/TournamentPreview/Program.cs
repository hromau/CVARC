using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using HoMM.World;
using Infrastructure;

namespace TournamentPreview
{
    class Program
    {
        static void Main(string[] args)
        {
            Environment.CurrentDirectory = @"C:\itplanetfinal\data";

            new SolutionsExtracter().Extract(new DirectoryInfo("C:\\itplanetfinal\\solutions"));
            new SolutionsExtracter().Control(new DirectoryInfo("C:\\itplanetfinal\\solutions"));
            new SolutionsExtracter().Print();
            new TournamentVerifier().VerifyAndSave("Final");
            TournamentVerifier.Print();
            new GroupCompetitionPlanner().Plan(
                GetLevel3GameSettings(),
                new[] {6, 6, 7},
                GetLevel3WorldStates(),
                142);
            using (var wr = new StreamWriter("groups.txt"))
                GroupCompetitionPlanner.Show(wr);
        }


        static void SomeOld(string[] args)
        {
            Environment.CurrentDirectory = @"C:\Solutions\commandFolder";
            //new GroupCompetitionPlanner().Plan(
            //    GetLevel3GameSettings(),
            //    Enumerable.Range(0, 9).Select(z => 5).ToArray(),
            //    GetLevel3WorldStates(),
            //    142);
            //using (var wr = new StreamWriter("groups.txt"))
            //    GroupCompetitionPlanner.Show(wr);


            using (var wr = new StreamWriter("groupResult.txt"))
                GroupCompetitionPlanner.ShowResults(wr);

            using (var wr = new StreamWriter("totalResult.txt"))
                GroupCompetitionPlanner.ShowGroupedResults(wr);
        }


        static void ForSingle()
        {
            //new SolutionsExtracter().Extract(new DirectoryInfo("C:\\Solutions"));
            //new SolutionsExtracter().Control(new DirectoryInfo("C:\\Solutions"));
            //new SolutionsExtracter().Print();
            //TournamentVerifier.Print();
            //new SingleRunCompetitionPlanner().Plan(GetLevel1GameSettings(), GetLevel1WorldStates());
            //SinglePlayReportMaker.MakeReport();
        }

        static object[] GetLevel1WorldStates()
        {
            return new object[]
            {
                new HommWorldState(123),
                new HommWorldState(124),
                new HommWorldState(125)
            };
        }


        static object[] GetLevel3WorldStates()
        {
            return Enumerable.Range(10, 7)
                .Select(z => new HommWorldState(z))
                .ToArray();
        }


        static GameSettings GetLevel1GameSettings()
        {
            var GameSettings = new GameSettings
            {
                LoadingData = new LoadingData
                {
                    AssemblyName = "homm",
                    Level = "level1"
                },
                ActorSettings = new List<ActorSettings>
                {
                    new ActorSettings
                    {
                        ControllerId = "Left",
                        IsBot = false,
                        PlayerSettings = new PlayerSettings
                        {
                            CvarcTag = Guid.Empty
                        }
                    }
                },
                SpectacularView = false,
                OperationalTimeLimit = 10,
                TimeLimit = 90,
                SpeedUp = true
            };
            return GameSettings;
        }


        static GameSettings GetLevel3GameSettings()
        {
            var GameSettings = new GameSettings
            {
                LoadingData = new LoadingData
                {
                    AssemblyName = "homm",
                    Level = "level3"
                },
                ActorSettings = new List<ActorSettings>
                {
                    new ActorSettings
                    {
                        ControllerId = "Left",
                        IsBot = false,
                        PlayerSettings = new PlayerSettings
                        {
                            CvarcTag = Guid.Empty
                        }
                    },
                    new ActorSettings
                    {
                        ControllerId = "Right",
                        IsBot = false,
                        PlayerSettings = new PlayerSettings
                        {
                            CvarcTag = Guid.Empty
                        }
                    }
                },
                SpectacularView = false,
                OperationalTimeLimit = 25,
                TimeLimit = 90,
                SpeedUp = true
            };
            return GameSettings;
        }
    }
}