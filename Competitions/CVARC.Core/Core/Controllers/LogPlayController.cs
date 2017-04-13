using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AIRLab;
using AIRLab.Mathematics;

namespace CVARC.V2
{
    public class LogPlayController : IController
    {
        List<ICommand> commands;
        IActor actor;
        int pointeer = 0;

        public LogPlayController(IEnumerable<ICommand> commands)
        {
            this.commands = commands.ToList();
        }

        public void Initialize(IActor controllableActor)
        {
            actor = controllableActor;
        }

        public ICommand GetCommand()
        {
            if (pointeer >= commands.Count)
                return null;
            var command=commands[pointeer];
            pointeer++;
            return command;
        }

        public void SendSensorData(object sensorData)
        {
           
        }

        public void SendError(Exception e)
        {
            
        }
    }
}