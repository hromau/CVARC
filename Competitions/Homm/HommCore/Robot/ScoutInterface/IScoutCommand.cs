using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HoMM.Robot.ScoutInterface
{
    interface IScoutCommand
    {
        ScoutOrder ScoutOrder { get; }
    }
}
