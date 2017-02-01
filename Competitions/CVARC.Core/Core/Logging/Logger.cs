using AIRLab.Mathematics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Infrastructure;

namespace CVARC.V2
{



    public class LogWriter
    {
        
        IWorld world;
        private bool enableLog;
        private string logFile;
        private Configuration configuration;
        private object worldState;

        public LogWriter(IWorld world, bool enableLog, string logFile, Configuration configuration, object worldState) 
        {
            this.world = world;
            this.enableLog = enableLog;
            this.logFile = logFile;
            this.configuration = configuration;
            this.worldState = worldState;
            
        }
        

        internal void AccountCommand(string controllerId, ICommand command)
        {
            throw new NotImplementedException();
        }
    }
}
