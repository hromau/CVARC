using System;
using Assets.Servers;
using Assets.Tools;
using CVARC.V2;
using Infrastructure;


namespace Assets
{
    public class GameManager
    {
        private PlayServer playServer;
        private LoadingData tutorialLoadingData;

        // еще сервер логов
        public GameManager()
        {
            playServer = new PlayServer(15000);
            // еще сервер логов
        }

        public bool CheckGame()
        {
            if (playServer.HasGame())
                return true;
            if (tutorialLoadingData != null)
                return true;

            return false;
        }

        public IWorld StartGame()
        {
            if (playServer.HasGame())
                return playServer.StartGame();
            if (tutorialLoadingData != null)
                return CreateTutorialGame();

            throw new Exception("game was not ready!");
        }

        public void EndGame(GameResult gameResult)
        {
            if (playServer.GameStarted)
                playServer.EndGame(gameResult);
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
    }
}
