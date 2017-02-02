using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure;

namespace CVARC.V2
{
    //NOT DONE!!
    public class MultiplayerProxy : IProxy
    {
        Dictionary<LoadingData, int> playerCountByLevel;
        Dictionary<LoadingData, List<CvarcClient>> playersByLevel;
        
        public MultiplayerProxy()
        {
            playerCountByLevel = new Dictionary<LoadingData, int>   //надо сделать, чтобы тянул из настоящих competititions
            {
                {new LoadingData {AssemblyName = "Pudge", Level = "Level1" }, 2},
                {new LoadingData {AssemblyName = "Pudge", Level = "Level2" }, 2},
                {new LoadingData {AssemblyName = "Pudge", Level = "Level3" }, 2}
            };
            playersByLevel = new Dictionary<LoadingData, List<CvarcClient>>();
        }

        public void AddPlayerAndCheck(CvarcClient client)
        {
            var loadingData = client.Read<LoadingData>();
            lock(playersByLevel)
            {
                if (!playersByLevel.ContainsKey(loadingData))
                    playersByLevel.Add(loadingData, new List<CvarcClient>());
                playersByLevel[loadingData].Add(client);
                if (playersByLevel[loadingData].Count == playerCountByLevel[loadingData])
                    TryConnectToServer(loadingData);
            }
        }

        public void TryConnectToServer(LoadingData level)
        {
            var currentPlayers = playersByLevel[level];
            foreach (var p in currentPlayers.ToArray())
                if (!p.IsAlive())
                    currentPlayers.Remove(p);
            if (currentPlayers.Count == playerCountByLevel[level])
            {
                //actually connect
            }
            throw new NotImplementedException();
        }

        public void PrepareConnection(string ip, int port, LoadingData loadingData, IWorldState worldState)
        {

        }
    }
}
