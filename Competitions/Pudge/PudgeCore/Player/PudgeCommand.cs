using System;
using System.Runtime.Serialization;
using AIRLab.Mathematics;
using Pudge.Units.DaggerUnit;
using Pudge.Units.HookUnit;
using Pudge.Units.WardUnit;
using Pudge.Units.WADUnit;

namespace Pudge.Player
{
    [Serializable]
    [DataContract]
    public class PudgeCommand : IGameCommand, IHookCommand, IWardCommand, IDaggerCommand
    {
        [DataMember]
        public GameMovement GameMovement{ get; set; }
        [DataMember]
        public bool MakeHook{ get; set; }
        [DataMember]
        public bool MakeWard{ get; set; }
        [DataMember]
        public DaggerDestinationPoint DaggerDestination { get; set; }
        [DataMember]
        public bool MakeDagger { get; set; }
    }
}