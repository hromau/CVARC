using System.Collections.Generic;
using System.Threading;
using Assets;
using Assets.Tools;
using CVARC.V2;

namespace Assets
{
    //Multiplayer pool
    public static class MultiplayerPool
    {
        static readonly Dictionary<LoadingData, MultiplayerRunner> pool = new Dictionary<LoadingData, MultiplayerRunner>();
        static bool havePriorityGame;
        static MultiplayerRunner priorityRunner;

        //-config proposal (only cvarctag)
        public static void AddPlayerToPool(CvarcClient client, Configuration configuration, IWorldState worldState, string cvarcTag)
        {
            if (havePriorityGame)
            {
                // сообщение тулзе о том, что игрок подключился.
                if (UnityConstants.TournamentToolMode)
                    HttpWorker.SendInfoToLocal(cvarcTag);
                if (priorityRunner.AddPlayerAndCheck(new MultiplayingPlayer(client, cvarcTag, worldState)))
                    havePriorityGame = false;
                return;
            }
            lock (pool)
            {
                if (!pool.ContainsKey(configuration.LoadingData))
                {
                    pool.Add(configuration.LoadingData, new MultiplayerRunner(worldState, configuration));
                    Dispatcher.AddRunner(pool[configuration.LoadingData]);
                }
                if (pool[configuration.LoadingData].AddPlayerAndCheck(new MultiplayingPlayer(client, cvarcTag, worldState)))
                    pool.Remove(configuration.LoadingData);
            }
        }

        public static void AddForceGame(IWorldState worldState, Configuration config)
        {
            havePriorityGame = true;
            lock (pool)
                pool.Clear();
            priorityRunner = new MultiplayerRunner(worldState, config);
            Dispatcher.AddRunner(priorityRunner);
        }
    }
}
