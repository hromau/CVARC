using Infrastructure;
using System;
using System.Net.Sockets;
using Newtonsoft.Json.Linq;

namespace CVARC.V2
{

    class ClientException  : Exception
    {
        public ClientException(Exception inner)
            : base("Connection error", inner)
        { }


        public ClientException(string serverError)
        : base("Error on server side: " + serverError)
        { }
    }

	public class CvarcClient<TSensorData, TCommand, TWorldState>
		where TSensorData : class
        where TWorldState : WorldState
	{
		TcpClient client;
        public event Action<TSensorData> OnSensorDataReceived;
        public event Action<string> OnError;
        public event Action<string> OnInfo;

        protected TSensorData Configurate(int port, GameSettings configuration, TWorldState state, string ip = "127.0.0.1")
		{
            client = new TcpClient();
		    try
		    {
		        client.Connect(ip, port);
		    }
		    catch (SocketException e)
		    {
		        if (OnError != null)
		            OnError("Cant connect to server. Run UnityStarter.bat first");
                throw new ClientException(e);
		    }
            

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

            var errorMessage = JObjectHelper.ParseSimple<string>(message.Message);
            if (OnError != null)
                OnError(errorMessage);
            throw new ClientException(errorMessage);
        }
        
		public TSensorData Act(TCommand command)
		{
			client.WriteJson(command);
            return ReadSensorData();
		}

		

		public void Exit()
		{
			client.Close();
		}
	}
}
