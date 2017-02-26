using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Infrastructure;
using System.Net.Sockets;
using Newtonsoft.Json.Linq;

namespace CVARC.V2
{

    public class NetworkController<TCommand> : INetworkController
        where TCommand : ICommand
    {
        TcpClient client;

        double OperationalTime;
        double OperationalTimeLimit;

        public void InitializeClient(TcpClient client)
        {
            this.client = client;
        }

        public void Initialize(IActor controllableActor)
        {
            OperationalTimeLimit = controllableActor.World.Configuration.OperationalTimeLimit;
        }

        PlayerMessage playerMessage;
        bool active = true;

        Tuple<ICommand, Exception> GetCommandInternally(Type commandType)
        {
            try
            {
                client.WriteJson(playerMessage);
                var command = (ICommand)client.ReadJson(commandType);
			    return new Tuple<ICommand,Exception>(command,null);
            }
            catch (Exception e)
            {
                return new Tuple<ICommand,Exception>(null,e);
            }
        }

        public ICommand GetCommand()
        {
            if (!active) return null;

            var @delegate = new Func<Type, Tuple<ICommand, Exception>>(GetCommandInternally);

            var async = @delegate.BeginInvoke(typeof(TCommand), null, null);

         
            while (OperationalTime <OperationalTimeLimit)
            {
                if (async.IsCompleted) break;
                OperationalTime += 0.001;
                Thread.Sleep(1);
            }

            if (OperationalTime < OperationalTimeLimit)
            {
                var result = @delegate.EndInvoke(async);
                if (result.Item2 != null)
                {
                    Debugger.Log("Exception: " + result.Item2.Message);
                    return null;
                }
                
                Debugger.Log("Command accepted in controller");
                return result.Item1;
            }

            Thread.Sleep(100);//without this sleep, if computer performs badly and units contain multiple triggers, the server will be stopped before test client receives data, hence client will throw exception.
       
			client.Close();
			Debugger.Log("Can't get command");
            return null;

        }

        public void SendSensorData(object sensorData)
        {
            this.playerMessage = new PlayerMessage
            {
                MessageType = MessageType.SensorData,
                Message = JObject.FromObject(sensorData)
            };
        }

        public void SendError(Exception e)
        {
            var msh = new PlayerMessage
            {
                MessageType = MessageType.Error,
                Message = JObjectHelper.CreateSimple<string>(e.GetType().Name + ": " + e.Message)
            };
            client.WriteJson(msh);
        }
    }
}
