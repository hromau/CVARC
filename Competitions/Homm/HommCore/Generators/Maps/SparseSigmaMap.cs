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
            return new SparseSigmaMap<TCell>(source.Size, i => source[i]);
        }

        public static SparseSigmaMap<TCell> From<TCell>(MapSize size, 
            Func<Location, TCell> cellsFactory, TCell defaultValue = default(TCell))
        {
            return new SparseSigmaMap<TCell>(size, cellsFactory);
        }
        
        public static ISigmaMap<TCell> Merge<TCell>(
            this ISigmaMap<TCell> bottom, ISigmaMap<TCell> top)
        {
            return SparseMerge(bottom, top);
        }

        public static ISigmaMap<TCell> SparseMerge<TCell>
            (this ISigmaMap<TCell> bottom, ISigmaMap<TCell> top)
        {
            if (bottom.Size != top.Size)
                throw new ArgumentException("Cannot merge maps of different size");

            return From(bottom.Size, s => top[s] == null ? bottom[s] : top[s]);
        }
    }
}
