namespace HoMM.Robot.ArmyInterface
{
    public class PurchaseOrder
    {
        public int Count { get; set; }

        public PurchaseOrder(int count)
        {
            Count = count;
        }

        public void Apply(Player player)
        {
            player.TryBuyUnits(Count);
        }
    }
}
