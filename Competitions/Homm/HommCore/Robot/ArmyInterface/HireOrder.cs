namespace HoMM.Robot.ArmyInterface
{
    public class HireOrder
    {
        public int Count { get; set; }

        public HireOrder(int count)
        {
            Count = count;
        }

        public void Apply(Player player)
        {
            player.TryBuyUnits(Count);
        }
    }
}
