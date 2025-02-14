﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace HoMM
{
    public static class RandomExtensions
    {
        public static T Choice<T>(this Random random, ICollection<T> source)
        {
            return source.Skip(random.Next(0, source.Count)).Single();
        }

        public static T Choice<T>(this Random random, params T[] source)
        {
            return source[random.Next(0, source.Length)];
        }

        public static T Choice<T>(this Random random, IList<T> source)
        {
            return source[random.Next(0, source.Count)];
        }

        public static T ChoiceWithProbability<T>(this Random random, double probability, T first, T second)
        {
            return random.NextDouble() < probability ? first : second;
        }

        public static TEnum Choice<TEnum>(this Random random)
        {
            var values = Enum.GetValues(typeof(TEnum));
            return (TEnum)values.GetValue(random.Next(0, values.Length));
        }
    }

    public static class DictionaryExtensions
    {
        public static TValue GetOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dict, 
            TKey key, TValue @default=default(TValue))
        {
            return (key != null && dict.ContainsKey(key)) ? dict[key] : @default;
        }
    }

    public static class EnumerableExtensions
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

        public static T Argmin<T>(this IEnumerable<T> source, Func<T, double> selector)
        {
            var min = double.MaxValue;
            var argmin = default(T);
            var empty = true;

            foreach (var element in source)
            {
                var valueToCompare = selector(element);

                if (valueToCompare < min)
                {
                    min = valueToCompare;
                    argmin = element;
                }

                empty = false;
            }

            if (empty)
                throw new ArgumentException($"{nameof(source)} is an empty sequence");

            return argmin;
        }
        
        public static T Argmax<T>(this IEnumerable<T> source, Func<T, double> selector)
        {
            var max = double.MinValue;
            var argmax = default(T);
            var empty = true;

            foreach (var element in source)
            {
                var valueToCompare = selector(element);

                if (valueToCompare > max)
                {
                    max = valueToCompare;
                    argmax = element;
                }

                empty = false;
            }
            
            if (empty)
                throw new ArgumentException($"{nameof(source)} is an empty sequence");

            return argmax;
        }
    }
}
