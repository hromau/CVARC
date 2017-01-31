using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using AIRLab;
using AIRLab.Mathematics;
using CVARC.V2;
using Infrastructure;


namespace Assets
{ //LogPlayerController CVARC.Unity.sln
    public class LogRunner : IRunner
    {
        readonly Configuration configuration;
        readonly IWorldState worldState;
        readonly LogPlayerControllerFactory factory;
        readonly double loggingDeltaTime;
        readonly List<Tuple<string, List<PositionLogItem>>> positions;
        const double TimeEpsilon = 0.02;

        public string Name { get; private set; }
        public IWorld World { get; private set; }
        public bool CanStart { get; private set; }
        public bool CanInterrupt { get; private set; }
        public bool Disposed { get; private set; }
        

        public LogRunner(Log log)
        {
            factory = new LogPlayerControllerFactory(log);
            configuration = log.Configuration;
            configuration.Settings.EnableLog = false; // чтоб файл логов не переписывать

            worldState = log.WorldState;
            loggingDeltaTime = log.LoggingDeltaTime;
            positions = log.Positions;

            CanInterrupt = true;
            CanStart = true;
            WriteLineToLogFile("start new session...");
        }

        public void InitializeWorld()
        {
            if (World == null)
            {
                World = Dispatcher.Loader.CreateWorld(configuration, factory, worldState);
                World.Clocks.AddTrigger(new TimerTrigger(UpdatePositions, loggingDeltaTime));
            }
        }

        public void UpdatePositions(double tick)
        {
            var engine = World.GetEngine<ICommonEngine>();
            WriteLineToLogFile(string.Format("time: {0:F}", tick));
            foreach (var movableActor in positions)
            {
                var curPosition = movableActor.Item2.FirstOrDefault(pos => Math.Abs(pos.Time - tick) < TimeEpsilon);
                if (curPosition == null || !curPosition.Exists) continue; // может проверять на контейн в мире (поздно подумал об этом)
                WriteLineToLogFile(movableActor.Item1);
                WriteLineToLogFile("logging: " + Prettify(curPosition.Location));
                WriteLineToLogFile("actual:  " + Prettify(engine.GetAbsoluteLocation(movableActor.Item1)));
                WriteLineToLogFile("");
                engine.SetAbsoluteLocation(movableActor.Item1, curPosition.Location);
            }
            WriteLineToLogFile("");
            WriteLineToLogFile("");
        }

        public void WriteLineToLogFile(string value)
        {
            if (UnityConstants.NeedToWritePositionsFromLogs)
                File.AppendAllText(UnityConstants.PathToFileWithPositions, value + "\n");
        }

        public string Prettify(Frame3D position)
        {
            return string.Format("X = {0:F}, Y = {1:F}, Angle = {2:F}", position.X, position.Y, position.Yaw.Grad);
        }

        public void Dispose()
        {
            if (World != null)
                World.OnExit();
            Disposed = true;
        }
    }
}
