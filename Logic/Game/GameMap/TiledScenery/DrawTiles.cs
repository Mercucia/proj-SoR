﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Graphics;
using MonoGame.Extended.Tiled;
using Spine;
using System.Collections.Generic;

namespace Logic.Game.GameMap.TiledScenery
{
    /*
     * Create a map using a specified tileset and layout.
     */
    internal class DrawTiles
    {
        private Texture2DAtlas floorAtlas;
        private Texture2DAtlas wallAtlas;
        private Dictionary<int, Texture2DRegion> floorTileSet;
        private Dictionary<int, Texture2DRegion> wallTileSet;
        private Texture2D floorTexture;
        private Texture2D wallTexture;
        private Rectangle targetRectangle;
        private string floorTiles;
        private string wallTiles;
        private int mapWidth;
        private int mapHeight;
        private int totalTiles;
        private int rowLength;
        private int columnLength;
        public Vector2 Position { get; set; }

        public DrawTiles(int map, string floorTileset, string wallTileset)
        {
            Position = Vector2.Zero;
            floorTiles = floorTileset;
            wallTiles = wallTileset;
        }

        /*
         * Load map content.
         */
        public void LoadMap(ContentManager Content, int rows, int columns)
        {
            mapWidth = 64 * rows;
            mapHeight = 64 * columns;
            totalTiles = rows * columns;
            rowLength = rows;
            columnLength = columns;
            

            targetRectangle = new Rectangle(-mapWidth / 2, -mapHeight / 2, mapWidth, mapHeight);

            floorTexture = Content.Load<Texture2D>(floorTiles);
            wallTexture = Content.Load<Texture2D>(wallTiles);

            floorAtlas = Texture2DAtlas.Create("background", floorTexture, 64, 64);
            wallAtlas = Texture2DAtlas.Create("foreground", wallTexture, 64, 64);

            CreateAtlas(23, floorTileSet, floorAtlas);
            CreateAtlas(5, wallTileSet, wallAtlas);
        }

        /*
         * Create an atlas from a spritesheet.
         */
        public void CreateAtlas(int tiles, Dictionary<int, Texture2DRegion> tileSet, Texture2DAtlas atlas)
        {
            for (int i = 0; i < 23; i++)
            {
                tileSet.Add(i, atlas[i]);
            }
        }

        /*
         * Get the next tile from the spritesheet. Try using instead of a new rectangle for each spritesheet region.
         */
        public Rectangle GetNextTile(int xStart, int yStart, int tileWidth, int tileHeight)
        {
            Rectangle rectangle = new Rectangle(xStart, yStart, tileWidth, tileHeight);

            return rectangle;
        }

        /*
         * Draw the map.
         */
        public void DrawMap(Texture2D tileSet, SpriteBatch spriteBatch, Vector2 location, int xStart, int yStart, int tileWidth, int tileHeight)
        {
            Rectangle sourceRectangle = new Rectangle(tileWidth * column, tileHeight * row, tileWidth, tileHeight);
            Rectangle destinationRectangle = new Rectangle((int)location.X, (int)location.Y, tileWidth, tileHeight);

            int width = TileSet.Width / Columns;
            int height = TileSet.Height / Rows;
            int row = currentTile / Columns;
            int column = currentTile % Columns;

            if (currentTile != totalTiles)
            {
                currentTile++;
            }

        }

        public void DebugMap(GraphicsDevice GraphicsDevice, int row = 0, int column = 1)
        {
            int rowNumber = 0;
            int columnNumber = 0;
            int previousColumn = -1;
            int previousRow = -1;

            for (int i = rowNumber; i < tileLocations.GetTempleLayout().GetLength(row); i++)
            {
                System.Diagnostics.Debug.Write(i + ": ");
                for (int j = columnNumber; j < tileLocations.GetTempleLayout().GetLength(column); j++)
                {
                    if (previousRow < 0 & previousColumn < 0)
                    {
                        System.Diagnostics.Debug.Write("First: ");
                    }
                    System.Diagnostics.Debug.Write(tileLocations.GetTempleLayout()[i, j] + ", ");

                    if (previousRow == i & previousColumn >= 0)
                    {
                        System.Diagnostics.Debug.Write("(" + tileLocations.GetTempleLayout()[i, j - 1] + "), ");
                    }
                    else if (previousRow - 1 < i & previousColumn == tileLocations.GetTempleLayout().GetLength(column) - 1)
                    {
                        System.Diagnostics.Debug.Write("(" + tileLocations.GetTempleLayout()[previousRow, previousColumn] + "), ");
                    }
                    previousColumn = j;
                    previousRow = i;
                }
                System.Diagnostics.Debug.Write("\n");
            }
        }

        /*
         * Get the target rectangle to render the map to.
         */
        public Rectangle GetTargetRectangle()
        {
            return targetRectangle;
        }
    }
}