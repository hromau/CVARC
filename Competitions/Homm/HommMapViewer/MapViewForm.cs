using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using HoMM.Generators;

namespace HoMM.MapViewer
{
    public partial class MapViewForm : Form
    {
        int diameter = 16;
        int mapSize = 18;
        
        public MapViewForm()
        {
            InitializeComponent();
            
            Size = new Size(1000, 700);
            WindowState = FormWindowState.Maximized;

            DoubleBuffered = true;

            var seed = 0;
            var r = new Random(seed);

            var easyTier = new SpawnerConfig(HoMM.Location.Zero, 3, 30, 0.5);
            var mediumTier = new SpawnerConfig(HoMM.Location.Zero, 30, 1000, 0.5);
            var hardTier = new SpawnerConfig(HoMM.Location.Zero, 14, 16, 0.5);
            var nightmare = new SpawnerConfig(HoMM.Location.Zero, 16.5, 20, 0.5);

            var gen = HommMapGenerator
                .From(new DiagonalMazeGenerator(r))
                .With(new BfsRoadGenerator(r, TileTerrain.Road)
                    .Over(new VoronoiTerrainGenerator(r, TileTerrain.Nature.ToArray())))
                .With(new GraphSpawner(r, mediumTier, p => new Mine(Resource.Crystals, p)))
                .With(new DistanceSpawner(r, hardTier, p => new Mine(Resource.Ore, p)))
                .With(new GraphSpawner(r, easyTier, p => new Mine(Resource.Rubles, p)))
                .And(new DistanceSpawner(r, nightmare, p => new Mine(Resource.Gems, p)));

            Map map = null;
            
            var generateButton = new Button { Text = "Generate!", Location = new Point(150, 0) };

            var mapSizeBox = new ComboBox();

            for (var size = 4; size < 20; ++size)
                mapSizeBox.Items.Add(2 * size);
            
            mapSizeBox.SelectedIndex = 5;

            generateButton.Click += (s, e) =>
            {
                mapSize = (int)mapSizeBox.SelectedItem;
                map = gen.GenerateMap(mapSize);
                this.Invalidate();
            };

            Controls.Add(mapSizeBox);
            Controls.Add(generateButton);

            Paint += (s, e) => {
                if (map != null)
                    foreach (var tile in map)
                        DrawTile(tile, e.Graphics);
            };
        }
        
        private void DrawTile(Tile cell, Graphics g)
        {
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            var dy = cell.location.X % 2 * 0.5f;
            var x = (cell.location.X + 4);
            var y = (cell.location.Y + 4);
            var voffset = mapSize + 2;
            var hoffset = mapSize + 2;

            var brush = new SolidBrush(GetColor(cell.tileObject, cell.tileTerrain, true, true, true));
            g.FillEllipse(brush, x*diameter,  (y+dy)*diameter, diameter, diameter);

            brush = new SolidBrush(GetColor(cell.tileObject, cell.tileTerrain, false, true, false));
            g.FillEllipse(brush, (x+hoffset)*diameter, (y+dy)*diameter, diameter, diameter);

            brush = new SolidBrush(GetColor(cell.tileObject, cell.tileTerrain, false, false, true));
            g.FillEllipse(brush, (x+2*hoffset)*diameter, (y+dy)*diameter, diameter, diameter);

            brush = new SolidBrush(GetColor(cell.tileObject, cell.tileTerrain, true, false, true));
            g.FillRectangle(brush, x * diameter, (y + voffset) * diameter, diameter, diameter);

            var ind = new Location(cell.location.Y, cell.location.X);
            var size = new MapSize(mapSize, mapSize);

            brush = new SolidBrush(ind.IsBelowDiagonal(size)
                ? Color.Red : (ind.IsAboveDiagonal(size) ? Color.Green : Color.Gray));
            g.FillEllipse(brush, (x + hoffset) * diameter, (y + dy + voffset) * diameter, diameter, diameter);
        }

        Dictionary<TileTerrain, Color> terrainColor = new Dictionary<TileTerrain, Color>
        {
            { TileTerrain.Arid, Color.Khaki },
            { TileTerrain.Desert, Color.LightGoldenrodYellow },
            { TileTerrain.Grass, Color.LightGreen },
            { TileTerrain.Marsh, Color.Pink },
            { TileTerrain.Road, Color.LightGray },
            { TileTerrain.Snow, Color.LightBlue }
        };

        Dictionary<Resource, Color> resourceColor = new Dictionary<Resource, Color>
        {
            { Resource.Rubles, Color.Green },
            { Resource.Crystals, Color.Blue },
            { Resource.Ore, Color.Red },
            { Resource.Gems, Color.Magenta },
        };

        private Color GetColor(TileObject obj, TileTerrain terrain, 
            bool drawObjects, bool drawWalls, bool drawTerrain)
        {
            if (obj as Mine != null && drawObjects)
                return resourceColor[(obj as Mine).Resource];

            if (obj != null && !obj.IsPassable && drawWalls)
                return Color.DarkSlateGray;
            
            if (terrainColor.ContainsKey(terrain) && drawTerrain)
                return terrainColor[terrain];

            return Color.Transparent;
        }
    }
}
