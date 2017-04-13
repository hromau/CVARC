using System;
using Assets.Servers;
using Assets.Tools;
using CVARC.V2;
using Infrastructure;
using System.Linq;

namespace Assets
{
    public class GameManager : IDisposable
    {
        private readonly PlayServer playServer;
        private readonly LogServer logServer;
        private LoadingData tutorialLoadingData;
        private GameSession gameSession;

        public GameManager()
        {
            playServer = new PlayServer(Constants.NetworkPort);
            if (!Settings.Current.ServerBuild)
                logServer = new LogServer(Constants.LogPort);
        }

        public RunType CheckGame()
        {
            if (playServer.HasGame())
                return RunType.Play;
            if (tutorialLoadingData != null)
                return RunType.Tutorial;
            if (logServer != null && logServer.HasGame())
                return RunType.Log;

            return RunType.NotReady;
        }

        public IWorld StartGame()
        {
            if (playServer.HasGame())
            {
                gameSession = playServer.StartGame();
                return gameSession.World;
            }
            if (tutorialLoadingData != null)
                return CreateTutorialGame();

            throw new Exception("game was not ready!");
        }

        public void EndGame(GameResult gameResult)
        {
            if (gameSession != null)
                gameSession.EndSession(gameResult);
            gameSession = null;
        }

        public void RequestTutorial(LoadingData loadingData)
        {
            tutorialLoadingData = loadingData;
        }

        private IWorld CreateTutorialGame()
        {
            var loadingData = tutorialLoadingData;
            tutorialLoadingData = null;
            var settings = DefaultWorldInfoCreator.GetDefaultGameSettings(loadingData);

            //заполняем ActorSettings, чтобы потом работал ReplayDebugger
            settings.ActorSettings = Dispatcher.Loader.GetCompetitions(loadingData).Logic.Actors.Keys.Select(z => new ActorSettings
            {
                IsBot = false,
                ControllerId = z,
                PlayerSettings=new PlayerSettings
                {
                     CvarcTag=Guid.Empty
                }
            }).ToList();

            return Dispatcher.Loader.CreateWorld(
                settings,
                new TutorialControllerFactory(),
                DefaultWorldInfoCreator.GetDefaultWorldState(loadingData));
        }

        public void Dispose()
        {
            playServer.Dispose();
            if (logServer != null)
                logServer.Dispose();
            if (gameSession != null)
                gameSession.EndSession(null);
        }
    }
}
