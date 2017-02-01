using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace HoMM.Generators
{
    abstract class ImmutableSigmaMap<TCell> : ISigmaMap<TCell>
    {
        public abstract TCell this[Location location] { get; }
        public abstract MapSize Size { get; }
        
        public IEnumerator<Location> GetEnumerator()
        {
            return Location
                .Square(Size)
                .Where(s => this[s] != null).GetEnumerator();
        }

        public ImmutableSigmaMap<TCell> Insert(Location location, TCell cell)
        {
            if (location.X < 0 || location.X > Size.X ||
                location.Y < 0 || location.Y > Size.Y)
                throw new ArgumentException("Modifying maze outside of its borders");

            return new ModifiedMapWrapper<TCell>(this, location, cell);
        }
        
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
