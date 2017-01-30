namespace Pudge.Units.PudgeUnit
{
    public interface IWADRules
    {
        double RotationVelocity{ get; }
        double MovementVelocity{ get; }
        double MovementRange{ get; }
        double RotationAngle{ get; }
        double WaitDuration{ get; }
    }
}