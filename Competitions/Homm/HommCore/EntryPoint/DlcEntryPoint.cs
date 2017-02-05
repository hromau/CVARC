using CVARC.V2;
using System.Collections.Generic;
using UnityCommons;

namespace HoMM
{
    public class DlcEntryPoint : IDlcEntryPoint
    {
        public IEnumerable<Competitions> GetLevels()
        {
            yield return new Competitions("Homm", "Level1", new HommLogicPartHelper(1),
                () => new UKeyboard(), HommEnginesMaker.CreateEngines);

            yield return new Competitions("Homm", "Level2", new HommLogicPartHelper(2),
                () => new UKeyboard(), HommEnginesMaker.CreateEngines);
        }
    }
}
