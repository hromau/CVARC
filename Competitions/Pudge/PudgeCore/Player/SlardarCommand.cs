using System;
using CVARC.V2;
using Pudge.Units.WADUnit;

namespace Pudge.Player
{
    public class SlardarCommand : ICommand, IGameCommand
    {
        public GameMovement GameMovement{ get; set; }
    }
}