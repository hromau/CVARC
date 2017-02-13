using CVARC.V2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HoMM.Engine
{
    enum UiLocation
    {
        Left, 
        Right,
    }

    interface IHommUserInterfaceEngine : IEngine
    {
        void InitUi(string id, UiLocation uiLocation);
        void UpdateResources(string id, Resource resource, int count);
        void UpdateArmy(string id, UnitType unit, int count);
    }
}
