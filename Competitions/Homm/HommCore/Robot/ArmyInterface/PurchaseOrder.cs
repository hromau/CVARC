namespace HoMM.Robot.ArmyInterface
{
    class PurchaseOrder : IOrder
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
