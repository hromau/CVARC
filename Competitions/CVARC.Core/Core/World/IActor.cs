﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CVARC.V2
{

    public interface IActor
    {
        string ObjectId { get; }
        bool IsDisabled { get; }
        IWorld World { get; }
        void Initialize(IWorld world, IRules rules,CommandFilterSet filters,  string objectId, string controllerId);
        string ControllerId { get; }
        void ExecuteCommand(ICommand command, out double commandDuration);
        object GetSensorData();
        IRules Rules { get; }
        ControlTrigger ControlTrigger { get; set; }
    }
}
