﻿using Logic.Game.GameMap;
using Logic.Game.GameMap.TiledScenery;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Graphics;
using SoR.Logic.Entities;
using Spine;
using System;
using System.Collections.Generic;

namespace Logic.Game.Graphics
{
    /*
     * Draw graphics to the screen.
     */
    internal partial class Render
    {
        private SpriteBatch spriteBatch;
        private SkeletonRenderer skeletonRenderer;
        public List<Rectangle> WalkableTiles { get; private set; }

        public Render(GraphicsDevice GraphicsDevice)
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            skeletonRenderer = new SkeletonRenderer(GraphicsDevice)
            {
                PremultipliedAlpha = true
            };

            WalkableTiles = new List<Rectangle>();
        }

        /*
         * Start drawing skeletons.
         */
        public void StartDrawingSkeleton(GraphicsDevice GraphicsDevice, Camera camera)
        {
            ((BasicEffect)skeletonRenderer.Effect).Projection = Matrix.CreateOrthographicOffCenter(
                    0,
                        GraphicsDevice.Viewport.Width,
                        GraphicsDevice.Viewport.Height,
                        0, 1, 0);
            ((BasicEffect)skeletonRenderer.Effect).View = camera.GetCamera().GetViewMatrix();

            skeletonRenderer.Begin();
        }

        /*
         * Start drawing SpriteBatch.
         */
        public void StartDrawingSpriteBatch(OrthographicCamera camera)
        {
            spriteBatch.Begin(transformMatrix: camera.GetViewMatrix(), samplerState: SamplerState.PointClamp, blendState: BlendState.AlphaBlend);
        }

        /*
         * Draw entity skeletons.
         */
        public void DrawEntitySkeleton(Entity entity)
        {
            // Draw skeletons
            skeletonRenderer.Draw(entity.GetSkeleton());
        }

        /*
         * Draw scenery skeletons.
         */
        public void DrawScenerySkeleton(Scenery scenery)
        {
            // Draw skeletons
            skeletonRenderer.Draw(scenery.GetSkeleton());
        }

        /*
         * Draw SpriteBatch for entities.
         */
        public void DrawEntitySpriteBatch(Entity entity, SpriteFont font)
        {
            // Entity text
            spriteBatch.DrawString(
                font,
                "HP: " + entity.GetHitPoints(),
                new Vector2(entity.GetPosition().X - 30, entity.GetPosition().Y + 30),
                Color.BlueViolet);
        }

        /*
         * Draw SpriteBatch for scenery.
         */
        public void DrawScenerySpriteBatch( Scenery scenery, SpriteFont font)
        {
            // Scenery text
            spriteBatch.DrawString(
                font,
                "",
                new Vector2(scenery.GetPosition().X - 80, scenery.GetPosition().Y + 100),
                Color.BlueViolet);
        }

        /*
         * Pair the atlas position of each tile with its world position.
         */
        public Dictionary<string, Vector2> CreateMap(Map map, int[,] tileLocations)
        {
            Dictionary<string, Vector2> sortByYAxis = [];
            Vector2 position = new(0, 0);
            int tileID = 1000;
            int row = 0;
            int column = 1;

            for (int x = 0; x < tileLocations.GetLength(row); x++) // For each row in the 2D array
            {
                for (int y = 0; y < tileLocations.GetLength(column); y++) // For each column
                {
                    int tile = tileLocations[x, y]; // Get the value of the current tile

                    if (tile > -1)
                    {
                        string tileName = string.Concat(tileID + tile.ToString());
                        sortByYAxis.Add(tileName, position);
                        tileID++;

                        Rectangle tileArea = new Rectangle ((int)position.X, (int)position.Y, map.Width, map.Height);
                        WalkableTiles.Add(tileArea);
                    }

                    position.X += map.Width; // Step right by one tile space
                }
                position.X = 0;
                position.Y += map.Height; // Reset the x-axis and step down by one tile space
            }

            return sortByYAxis;
        }

        /*
         * Create a list of rectangles containing the walkable map area.
         */
        public List<Rectangle> WalkableMapArea()
        {
            List<Rectangle> walkableArea = [];
            Rectangle block = new();
            int yPrev = 0;

            foreach (Rectangle area in WalkableTiles)
            {
                block.Y = area.Y; // Save the y position
                block.Height = area.Height; // save the height

                // If the previous y position is the same as the new one, and
                // the right x pos of the block being created is not less than the new left x pos
                if (yPrev == area.Y && block.X + block.Width + 1 !< area.X)
                {
                    block.Width += area.Width; // add the new width to that of the block being created
                }
                else // otherwise
                {
                    walkableArea.Add(block); // add this block to walkableArea array
                    block.X = area.X; // save the x position
                    block.Width = area.Width; // set the width to that of the new block width
                }

                yPrev = area.Y; // Save the previous y position
            }

            /*
             * for each area in walkable
             * save 
             */

            return walkableArea;
        }

        /*
         * Draw the map to the screen.
         */
        public void DrawMap(Texture2DAtlas atlas, Map map, string tileName, Vector2 position, SpriteFont font)
        {
            string tile = tileName.Remove(0, 4);

            int tileNumber = Convert.ToInt32(tile);

            // Offset drawing position by tile height to draw in front of the components that use a different positioning reference
            position.Y -= (float)(map.Height * 1.25);

            spriteBatch.Draw(atlas[tileNumber], position, Color.White);
        }

        /*
         * Finish drawing Skeleton.
         */
        public void FinishDrawingSkeleton()
        {
            skeletonRenderer.End();
        }

        /*
         * Finish drawing SpriteBatch.
         */
        public void FinishDrawingSpriteBatch()
        {
            spriteBatch.End();
        }
    }
}