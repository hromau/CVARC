using Infrastructure;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace CVARC.V2
{
	public class CvarcClient<TSensorData, TCommand, TWorldState>
		where TSensorData : class
        where TWorldState : IWorldState
	{
		TcpClient client;



		public TSensorData Configurate(int port, GameSettings configuration, TWorldState state, string ip = "127.0.0.1")
		{
            client = new TcpClient();
            client.Connect(ip, port);

			client.WriteJson(configuration);
			client.WriteJson(state);
            var sensorData=client.ReadJson<TSensorData>();
			OnSensorDataReceived(sensorData);
			return sensorData;
		}



		public TSensorData Act(TCommand command)
		{
			client.WriteJson(command);
			var sensorData = client.ReadJson<TSensorData>(); // 11!!!
			//if (sensorData == null)
			//	Environment.Exit(0);
				OnSensorDataReceived(sensorData);
			return sensorData;
		}

		public event Action<TSensorData> SensorDataReceived;
		void OnSensorDataReceived(TSensorData sensorData)
		{
			if (SensorDataReceived!=null)
				SensorDataReceived(sensorData);
		}

		public void Exit()
		{
			client.Close();
		}
	}
}
