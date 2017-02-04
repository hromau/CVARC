using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using HoMM.World;

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

            var generator = new MapGeneratorHelper().CreateGenerator(r);

            Map map = null;
            
            var generateButton = new Button { Text = "Generate!", Location = new Point(150, 0) };

            var mapSizeBox = new ComboBox();

            for (var size = 4; size < 20; ++size)
                mapSizeBox.Items.Add(2 * size);
            
            mapSizeBox.SelectedIndex = 5;

            generateButton.Click += (s, e) =>
            {
                mapSize = (int)mapSizeBox.SelectedItem;
                map = generator.GenerateMap(mapSize);
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
            var dy = cell.Location.X % 2 * 0.5f;
            var x = (cell.Location.X + 4);
            var y = (cell.Location.Y + 4);
            var voffset = mapSize + 2;
            var hoffset = mapSize + 2;

            var brush = new SolidBrush(GetColor(cell.Objects, cell.Terrain, drawTerrain: true));
            g.FillEllipse(brush, x * diameter, (y + dy) * diameter, diameter, diameter);

            brush = new SolidBrush(GetColor(cell.Objects, cell.Terrain, drawTerrain: true, drawWalls: true));
            g.FillEllipse(brush, (x + hoffset) * diameter, (y + dy) * diameter, diameter, diameter);

            brush = new SolidBrush(GetColor(cell.Objects, cell.Terrain, drawEnemies: true, drawWalls: true));
            g.FillEllipse(brush, (x + 2 * hoffset) * diameter, (y + dy) * diameter, diameter, diameter);

            brush = new SolidBrush(GetColor(cell.Objects, cell.Terrain, drawPiles: true, drawWalls: true));
            g.FillEllipse(brush, x * diameter, (y + dy + voffset) * diameter, diameter, diameter);

            brush = new SolidBrush(GetColor(cell.Objects, cell.Terrain, drawMines: true, drawWalls: true));
            g.FillEllipse(brush, (x + hoffset) * diameter, (y + dy + voffset) * diameter, diameter, diameter);

            brush = new SolidBrush(GetColor(cell.Objects, cell.Terrain, drawDwellings: true, drawWalls: true));
            g.FillEllipse(brush, (x + 2*hoffset) * diameter, (y + dy + voffset) * diameter, diameter, diameter);
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
            
            { Resource.Horses, Color.DarkGray },
            { Resource.Ore, Color.Cyan },
            { Resource.Gold, Color.Yellow },
            { Resource.Wood, Color.Brown }
        };

        Dictionary<UnitType, Color> dwellingColor = new Dictionary<UnitType, Color>
        {
            { UnitType.Cavalry, Color.DarkGray },
            { UnitType.Infantry, Color.Cyan },
            { UnitType.Militia, Color.Yellow },
            { UnitType.Ranged, Color.Brown }
        };

        private Color GetColor(List<TileObject> objects, TileTerrain terrain,
            bool drawWalls = false,
            bool drawTerrain = false,
            bool drawPiles = false,
            bool drawDwellings = false,
            bool drawMines = false,
            bool drawEnemies = false)
        {
            foreach (var obj in objects)
            {
                if (obj is Mine && drawMines)
                    return resourceColor[(obj as Mine).Resource];

                if (obj is ResourcePile && drawPiles)
                    return resourceColor[(obj as ResourcePile).Resource];

                if (obj is Dwelling && drawDwellings)
                    return dwellingColor[(obj as Dwelling).Recruit.UnitType];

                if (obj is NeutralArmy && drawEnemies)
                    return Color.FromArgb(255 - (int)(((NeutralArmy)obj).Army.Values.First() / 50.0 * 255), 0, 0);

                if (obj != null && !obj.IsPassable && drawWalls)
                    return Color.DarkSlateGray;
            }

            if (terrainColor.ContainsKey(terrain) && drawTerrain)
                return terrainColor[terrain];

            return Color.Transparent;
        }
    }
}
