using System;
using System.Collections.Generic;

namespace CVARC.V2
{
    public abstract class SettingsProposalMutator : Attribute
    {
        public SettingsProposal Mutate(SettingsProposal settings)
        {
            return MutateCopy(SettingsProposal.DeepCopy(settings));
        }

        protected abstract SettingsProposal MutateCopy(SettingsProposal settings);
    }

    public class TestSettings
    {
        [AttributeUsage(AttributeTargets.Method)]
        public class Name : SettingsProposalMutator
        {
            private readonly string _name;

            public Name(string name)
            {
                _name = name;
            }

            protected override SettingsProposal MutateCopy(SettingsProposal settings)
            {
                settings.Name = _name;
                return settings;
            }
        }

        [AttributeUsage(AttributeTargets.Method)]
        public class TimeLimit : SettingsProposalMutator
        {
            private readonly double _timeLimit;

            public TimeLimit(double timeLimit)
            {
                _timeLimit = timeLimit;
            }

            protected override SettingsProposal MutateCopy(SettingsProposal settings)
            {
                settings.TimeLimit = _timeLimit;
                return settings;
            }
        }

        [AttributeUsage(AttributeTargets.Method)]
        public class EnableLog : SettingsProposalMutator
        {
            private readonly bool _enableLog;

            public EnableLog(bool enableLog)
            {
                _enableLog = enableLog;
            }

            protected override SettingsProposal MutateCopy(SettingsProposal settings)
            {
                settings.EnableLog = _enableLog;
                return settings;
            }
        }

        [AttributeUsage(AttributeTargets.Method)]
        public class LogFile : SettingsProposalMutator
        {
            private readonly string _logFile;

            public LogFile(string logFile)
            {
                _logFile = logFile;
            }

            protected override SettingsProposal MutateCopy(SettingsProposal settings)
            {
                settings.LogFile = _logFile;
                return settings;
            }
        }

        [AttributeUsage(AttributeTargets.Method)]
        public class SpeedUp : SettingsProposalMutator
        {
            private readonly bool _speedUp;

            public SpeedUp(bool speedUp)
            {
                _speedUp = speedUp;
            }

            protected override SettingsProposal MutateCopy(SettingsProposal settings)
            {
                settings.SpeedUp = _speedUp;
                return settings;
            }
        }

        [AttributeUsage(AttributeTargets.Method)]
        public class OperationalTimeLimit : SettingsProposalMutator
        {
            private readonly double _operationalTimeLimit;

            public OperationalTimeLimit(double operationalTimeLimit)
            {
                _operationalTimeLimit = operationalTimeLimit;
            }

            protected override SettingsProposal MutateCopy(SettingsProposal settings)
            {
                settings.OperationalTimeLimit = _operationalTimeLimit;
                return settings;
            }
        }

        [AttributeUsage(AttributeTargets.Method)]
        public class Port : SettingsProposalMutator
        {
            private readonly int _port;

            public Port(int port)
            {
                _port = port;
            }

            protected override SettingsProposal MutateCopy(SettingsProposal settings)
            {
                settings.Port = _port;
                return settings;
            }
        }

        [AttributeUsage(AttributeTargets.Method)]
        public class SolutionsFolder : SettingsProposalMutator
        {
            private readonly string _solutionsFolder;

            public SolutionsFolder(string solutionsFolder)
            {
                _solutionsFolder = solutionsFolder;
            }

            protected override SettingsProposal MutateCopy(SettingsProposal settings)
            {
                settings.SolutionsFolder = _solutionsFolder;
                return settings;
            }
        }

        [AttributeUsage(AttributeTargets.Method)]
        public class Controllers : SettingsProposalMutator
        {
            private readonly List<ControllerSettings> _controllers;

            public Controllers(List<ControllerSettings> controllers)
            {
                _controllers = controllers;
            }

            protected override SettingsProposal MutateCopy(SettingsProposal settings)
            {
                settings.Controllers = _controllers;
                return settings;
            }
        }

        [AttributeUsage(AttributeTargets.Method)]
        public class SetReflected : Attribute
        {
            public readonly bool Reflected;

            public SetReflected(bool reflected)
            {
                Reflected = reflected;
            }
        }

        [AttributeUsage(AttributeTargets.Method)]
        public class WorldState : Attribute
        {
            public readonly IWorldState State;

            public WorldState(IWorldState state)
            {
                State = state;
            }
        }
    }

}
