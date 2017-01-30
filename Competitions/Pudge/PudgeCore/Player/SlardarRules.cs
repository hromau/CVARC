using AIRLab.Mathematics;
using CVARC.V2;
using Pudge.Units.PudgeUnit;

namespace Pudge.Player
{
    public class SlardarRules : IWADRules, IRules
    {
        public static SlardarRules Current = new SlardarRules();

        public Frame3D LeftSlardarPosition { get { return new Frame3D(-140, 60, 0);} }
        public Frame3D RightSlardarPosition { get { return new Frame3D(140, -60, 0);} }
        public double WaitDuration { get { return 0.1; } }
        public double RotationVelocity { get { return 360; } }

        public double MovementVelocity { get { return 40; } }

        public double RotationAngle { get { return 5; } }

        public double MovementRange { get { return 10; } }
        public double ForwardVisibilityRadius{ get { return PudgeRules.Current.VisibilityRadius * 0.85; } }
        public double SideVisibilityRadius { get { return ForwardVisibilityRadius * 0.65; } }

        public void DefineKeyboardControl(IKeyboardController keyboardController, string controllerId)
        {
            
        }
    }
}