namespace Pudge.Units.WardUnit
{
    public interface IWardRules
    {
        double WardDuration{ get; }
        double WardRadius{ get; }
        double WardIncrementTime{ get; }
        int AvailableWardsAtStart{ get; }
    }
}