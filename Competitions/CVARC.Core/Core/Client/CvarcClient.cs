using Infrastructure;
using System;
using System.Net.Sockets;
using Newtonsoft.Json.Linq;

namespace CVARC.V2
{
	public class CvarcClient<TSensorData, TCommand, TWorldState>
		where TSensorData : class
        where TWorldState : WorldState
	{
		TcpClient client;
	    private bool errorHappend;
        public event Action<TSensorData> OnSensorDataReceived;
        public event Action<string> OnError;
        public event Action<string> OnInfo;

        protected TSensorData Configurate(int port, GameSettings configuration, TWorldState state, string ip = "127.0.0.1")
		{
            client = new TcpClient();
            client.Connect(ip, port);

			client.WriteJson(configuration);
			client.WriteJson(JObject.FromObject(state));
            return ReadSensorData();
		}

	    private TSensorData ReadSensorData()
	    {
            var message = client.ReadJson<PlayerMessage>();

            while (message.MessageType == MessageType.Info)
            {
                if (OnInfo != null)
                    OnInfo(JObjectHelper.ParseSimple<string>(message.Message));
                message = client.ReadJson<PlayerMessage>();
            }

            if (message.MessageType == MessageType.SensorData)
            {
                var sensorData = message.Message.ToObject<TSensorData>();
                if (OnSensorDataReceived != null)
                    OnSensorDataReceived(sensorData);
                return sensorData;
            }
            if (OnError != null)
                OnError(JObjectHelper.ParseSimple<string>(message.Message));
            errorHappend = true;
            return null;
        }
        
		public TSensorData Act(TCommand command)
		{
		    if (errorHappend)
		        return null;

			client.WriteJson(command);
            return ReadSensorData();
		}

		

		public void Exit()
		{
			client.Close();
		}
	}
}
