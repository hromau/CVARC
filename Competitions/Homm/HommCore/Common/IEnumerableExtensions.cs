using HoMM.Generators;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HoMM
{
    public static class IEnumerableExtensions
    {
        /// <summary>
        /// Yields only indices that are lying inside of a triangle with 
        /// sides produced by lines: X = 0; Y = 0; X / size.X + Y / size.Y = 1
        /// </summary>
        public static IEnumerable<Location> InsideAndAboveDiagonal
            (this IEnumerable<Location> source, MapSize size)
        {
            return source
                .Where(location => location.IsInside(size) && location.IsAboveDiagonal(size));
        }

        public static IEnumerable<Location> Inside
            (this IEnumerable<Location> source, MapSize size)
        {
            return source
                .Where(location => location.IsInside(size));
        }

        public static T Argmin<T>(this ICollection<T> source, Func<T, double> selector)
        {
            var min = source.Min(selector);
            return source.Where(x => selector(x) == min).First();
        }
        
        public static T Argmax<T>(this ICollection<T> source, Func<T, double> selector)
        {
            var max = source.Max(selector);
            return source.Where(x => selector(x) == max).First();
        }
    }
}
