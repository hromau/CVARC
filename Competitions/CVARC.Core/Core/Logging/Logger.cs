using AIRLab.Mathematics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using AIRLab;

namespace CVARC.V2
{
    [Serializable]
     public class PositionLogItem
    {
        public double Time { get; set; }
        public Frame3D Location { get; set; }
        public bool Exists { get; set; }
    }



    public class Logger
    {
        public double LoggingDeltaTime = 0.1;
        public string LogFileName { get; set; }
        public bool SaveLog { get; set; }
        public Log Log { get; private set; }
        
        IWorld world;
        public Logger(IWorld world)
        {
            this.world = world;
            Log=new V2.Log();
            Log.LoggingDeltaTime = LoggingDeltaTime;
            world.Clocks.AddTrigger(new TimerTrigger(UpdatePositions, LoggingDeltaTime));
            world.Exit += world_Exit;
        }

        void world_Exit()
        {
            if (!SaveLog) return;
            string filename=world.Configuration.Settings.LogFile;
            if (filename == null)
            {
                for (int i = 0; ; i++)
                {
                    filename = "log" + i + ".cvarclog";
                    if (!File.Exists(filename))
                        break;
                }
            }
            //Log.Save(filename);
        }

        public void AccountCommand(string controllerId, ICommand command)
        {
            if (!Log.Commands.Any(x => x.Item1 == controllerId))
                return;
            Log.Commands.First(x => x.Item1 == controllerId).Item2.Add(command);
        }

        public void AddId(string controllerId)
        {
            if (!Log.Commands.Any(x => x.Item1 == controllerId))
            {
                Log.Commands.Add(new Tuple<string, List<ICommand>>(controllerId, new List<ICommand>()));
            }
        }

        void UpdatePositions(double tick)
        {
            var engine = world.GetEngine<ICommonEngine>();
            foreach (var e in world.LoggingPositionObjectIds)
            {
                if (!Log.Positions.Any(x => x.Item1 == e))
                    Log.Positions.Add(new Tuple<string, List<PositionLogItem>>(e, new List<PositionLogItem>()));
                var item = new PositionLogItem { Time = tick };
                if (!engine.ContainBody(e))
                    item.Exists = false;
                else
                {
                    item.Exists = true;
                    item.Location = engine.GetAbsoluteLocation(e);
                }
                Log.Positions.First(x => x.Item1 == e).Item2.Add(item);
            }
        }


    }
}
