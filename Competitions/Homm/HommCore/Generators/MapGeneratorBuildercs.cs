using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HoMM.Generators
{
    public partial class HommMapGenerator
    {
        public static BuilderOnSelectTerrain From(IMazeGenerator mazeGenerator)
        {
            return new BuilderOnSelectTerrain(mazeGenerator);
        }

        public class BuilderOnSelectTerrain
        {
            IMazeGenerator mazeGenerator;

            internal BuilderOnSelectTerrain(IMazeGenerator mazeGenerator)
            {
                this.mazeGenerator = mazeGenerator;
            }

            public BuilderOnSelectEntities With(ITerrainGenerator terrainGenerator)
            {
                return new BuilderOnSelectEntities(mazeGenerator, terrainGenerator);
            }

            public HommMapGenerator And(ITerrainGenerator terrainGenerator)
            {
                return new HommMapGenerator(mazeGenerator, terrainGenerator);
            }
        }

        public class BuilderOnSelectEntities
        {
            IMazeGenerator mazeGenerator;
            ITerrainGenerator terrainGenerator;
            List<ISpawner> entitiesGenerators = new List<ISpawner>();

            internal BuilderOnSelectEntities(
                IMazeGenerator mazeGenerator, ITerrainGenerator terrainGenerator)
            {
                this.mazeGenerator = mazeGenerator;
                this.terrainGenerator = terrainGenerator;
            }
            
            public BuilderOnSelectEntities With(ISpawner entitiesGenerator)
            {
                entitiesGenerators.Add(entitiesGenerator);
                return this;
            }

            public HommMapGenerator And(ISpawner entitiesGenerator)
            {
                With(entitiesGenerator);
                return new HommMapGenerator(mazeGenerator, terrainGenerator, 
                    entitiesGenerators.ToArray());
            }
        }
    }
}
