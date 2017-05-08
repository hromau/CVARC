namespace HoMM.ClientClasses
{
    public class RoughQuantity
    {
        public int From { get; }
        public int To { get; }

        public RoughQuantity(int from, int to)
        {
            From = from;
            To = to;
        }

        public override string ToString()
        {
            return $"{From}-{To}";
        }

        public override bool Equals(object obj)
        {
            if (!(obj is RoughQuantity))
                return false;

            var other = (RoughQuantity) obj;

            return other.From == From && other.To == To;
        }

        public override int GetHashCode()
        {
            return From + 31 * To;
        }
    }
}