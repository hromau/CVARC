namespace Pudge.Units.PudgeUnit
{
    public static class IWADRulesExtensions
    {
        public static double GetMovementDuration(this IWADRules rules)
        {
            return rules.MovementRange/rules.MovementVelocity;
        }
        public static double GetRotationDuration(this IWADRules rules)
        {
            return rules.RotationAngle / rules.RotationVelocity;
        }
    }
}