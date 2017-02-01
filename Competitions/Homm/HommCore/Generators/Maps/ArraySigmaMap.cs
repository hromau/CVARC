using System;

namespace HoMM.Generators
{
    class ArraySigmaMap<TCell> : ImmutableSigmaMap<TCell>
    {
        private TCell[,] cells;

        public override TCell this[Location location]
        {
            get { return cells[location.Y, location.X]; }
        }

        public override MapSize Size {
            get { return new MapSize(cells.GetLength(0), cells.GetLength(1)); }
        }

        public ArraySigmaMap(MapSize size, Func<Location, TCell> cellsFactory)
        {
            cells = new TCell[size.Y, size.X];

            foreach (var location in Location.Square(size))
                cells[location.Y, location.X] = cellsFactory(location);
        }
    }

    static class ArraySigmaMap
    {
        public static ArraySigmaMap<TCell> From<TCell>(MapSize size, Func<Location, TCell> cellsFactory)
        {
            return new ArraySigmaMap<TCell>(size, cellsFactory);
        }

        public static ArraySigmaMap<TCell> From<TCell>(ISigmaMap<TCell> source)
        {
            return From(source.Size, location => source[location]);
        }
        
        public static ISigmaMap<TCell> ArrayMerge<TCell>
            (this ISigmaMap<TCell> bottom, ISigmaMap<TCell> top)
        {
            if (bottom.Size != top.Size)
                throw new ArgumentException("Cannot merge maps of different size");

            return From(bottom.Size, s => top[s] == null ? bottom[s] : top[s]);
        }
    }
}
