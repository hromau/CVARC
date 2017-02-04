using System;
using Assets.Servers;
using Assets.Tools;
using CVARC.V2;
using Infrastructure;


namespace Assets
{
    public class GameManager : IDisposable
    {
        private PlayServer playServer;
        private LogServer logServer;
        private LoadingData tutorialLoadingData;
        private GameSession gameSession;

        // еще сервер логов
        public GameManager()
        {
            playServer = new PlayServer(UnityConstants.NetworkPort);
            logServer = new LogServer(UnityConstants.LogPort);
            // еще сервер логов
        }

        public RunType CheckGame()
        {
            if (playServer.HasGame())
                return RunType.Play;
            if (tutorialLoadingData != null)
                return RunType.Tutorial;
            if (logServer.HasGame())
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
            return Dispatcher.Loader.CreateWorld(
                DefaultWorldInfoCreator.GetDefaultGameSettings(loadingData),
                new TutorialControllerFactory(),
                DefaultWorldInfoCreator.GetDefaultWorldState(loadingData));
        }

        public void Dispose()
        {
            playServer.Dispose();
            logServer.Dispose();
        }
    }
}
