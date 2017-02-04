using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HoMM.Generators
{
    class SparseSigmaMap<TCell> : ImmutableSigmaMap<TCell>
    {
        public override MapSize Size { get; }

        private IDictionary<Location, TCell> cells;
        private TCell defaultValue;

        public SparseSigmaMap(MapSize size, IDictionary<Location, TCell> cells, 
            TCell defaultValue = default(TCell))
        {
            Size = size;
            this.cells = cells;
            this.defaultValue = defaultValue;
        }

        public SparseSigmaMap(MapSize size, Func<Location, TCell> cellsFactory,
            TCell defaultValue = default(TCell))
            : this(size, new Dictionary<Location, TCell>(), defaultValue)
        {
            foreach (var location in Location.Square(size))
            {
                var cell = cellsFactory(location);
                
                if (cell == null || !cell.Equals(defaultValue))
                    cells.Add(location, cell);
            }
        }

        public override TCell this[Location location]
        {
            get { return cells.ContainsKey(location) ? cells[location] : defaultValue; }
        }
    }

    static class SparseSigmaMap
    {
        public static SparseSigmaMap<TCell> From<TCell>(ISigmaMap<TCell> source, 
            TCell defaultValue = default(TCell))
        {
            return new SparseSigmaMap<TCell>(source.Size, i => source[i], defaultValue);
        }

        public static SparseSigmaMap<TCell> From<TCell>(MapSize size, 
            Func<Location, TCell> cellsFactory, TCell defaultValue = default(TCell))
        {
            return new SparseSigmaMap<TCell>(size, cellsFactory, defaultValue);
        }
        
        public static ISigmaMap<TRes> Merge<TLeft, TRight, TRes>(
            this ISigmaMap<TLeft> left, ISigmaMap<TRight> right, Func<TLeft, TRight, TRes> mergeFunc)
        {
            return SparseMerge(left, right, mergeFunc);
        }

        public static ISigmaMap<TRes> SparseMerge<TLeft, TRight, TRes>
            (this ISigmaMap<TLeft> left, ISigmaMap<TRight> right, Func<TLeft, TRight, TRes> mergeFunc)
        {
            if (left.Size != right.Size)
                throw new ArgumentException("Cannot merge maps of different size");

            return From(left.Size, s => mergeFunc(left[s], right[s]));
        }
    }
}
