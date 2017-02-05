namespace HoMM.Units.ArmyInterface
{
    class PurchaseOrder : IOrder
    {
        public int Count { get; }

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
