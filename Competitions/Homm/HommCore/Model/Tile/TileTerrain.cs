using System;
using System.Collections.Generic;
using System.Linq;

namespace HoMM
{
    public class TileTerrain
    {
        public static readonly TileTerrain Road = new TileTerrain(0.75);
        public static readonly TileTerrain Grass = new TileTerrain(1);
        public static readonly TileTerrain Snow = new TileTerrain(1.3);
        public static readonly TileTerrain Desert = new TileTerrain(1.15);
        public static readonly TileTerrain Marsh = new TileTerrain(1.3);

        public static readonly IEnumerable<TileTerrain> Nature = new TileTerrain[]
        {
            Grass, Snow, Desert, Marsh
        };

        public double TravelCost { get; private set; }

        private TileTerrain(double travelCost)
        {
            TravelCost = travelCost;
        }

        static Dictionary<char, TileTerrain> terrainParser = new Dictionary<char, TileTerrain>
        {
            { 'D', Desert },
            { 'G', Grass },
            { 'M', Marsh },
            { 'R', Road },
            { 'S', Snow }
        };
        
        public static TileTerrain Parse(char c)
        {
            if (terrainParser.ContainsKey(c)) return terrainParser[c];
            throw new ArgumentException("Unknown terrain type!");
        }

        static Dictionary<TileTerrain, char> terrainEncoder = terrainParser
            .ToDictionary(kv => kv.Value, kv => kv.Key);

        public char ToChar()
        {
            return terrainEncoder[this];
        }
    }
}
