namespace HoMM.Generators
{
    public interface ITerrainGenerator
    {
        ISigmaMap<TileTerrain> Construct(ISigmaMap<MazeCell> maze);
    }

    public static class ITerrainGeneratorExtension
    {
        public static ITerrainGenerator Over(this ITerrainGenerator top, ITerrainGenerator bottom)
        {
            return new AggregatedTerrainGenerator(top, bottom);
        }
    }
}
