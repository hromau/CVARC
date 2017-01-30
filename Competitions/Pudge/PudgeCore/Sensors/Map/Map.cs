using System.Collections.Generic;
using System.Runtime.Serialization;
using AIRLab.Mathematics;
using Pudge.ClientClasses;

namespace Pudge.Sensors.Map
{

    public enum HeroType
    {
        Pudge,
        Slardar
    }

    public class InternalHeroData
    {
        public Frame3D Location{ get; set; }
        public HeroType Type{ get; set; }
    }

    [DataContract]
    public class Map
    {
        public Map(List<HeroData> heroDatas, List<RuneData> runeDatas, List<HookData> enemyHooks)
        {
            Runes = runeDatas;
            Heroes = heroDatas;
            EnemyHooks = enemyHooks;
        }
        [DataMember]
        public List<RuneData> Runes { get; private set; }
        [DataMember]
        public List<HeroData> Heroes{ get; private set; } 
        [DataMember]
        public List<HookData> EnemyHooks{ get; private set; }
    }
}