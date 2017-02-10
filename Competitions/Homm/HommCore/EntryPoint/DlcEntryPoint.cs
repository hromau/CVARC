using CVARC.V2;
using System.Collections.Generic;
using UnityCommons;

namespace HoMM
{
    public class DlcEntryPoint : IDlcEntryPoint
    {
        public IEnumerable<Competitions> GetLevels()
        {
            yield return new Competitions("homm", "level1", new HommLogicPartHelper(1),
                () => new UKeyboard(), HommEnginesMaker.CreateEngines);

            yield return new Competitions("homm", "level2", new HommLogicPartHelper(2),
                () => new UKeyboard(), HommEnginesMaker.CreateEngines);
        }
    }
}
