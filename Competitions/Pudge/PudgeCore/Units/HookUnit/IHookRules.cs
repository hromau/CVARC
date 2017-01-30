namespace Pudge.Units.HookUnit
{
    public interface IHookRules
    {
        double HookVelocity{ get; }
        double HookDuration{ get; }
        double HookRange{ get; }
        double HookCooldown{ get; }
        double HookAttackRadius { get; }
    }
}