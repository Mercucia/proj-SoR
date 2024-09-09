﻿using Logic.Game.Settings;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.ViewportAdapters;
using System;

namespace Logic.Game
{
    /*
     * The game camera, which follows the player.
     */
    public class Camera
    {
        private OrthographicCamera camera;
        private BoxingViewportAdapter viewportAdapter;
        private Matrix viewMatrix;
        private KeyboardState keyState;
        private KeyboardState lastKeyState;
        private bool resolutionChange;
        private int screenWidth;
        private int screenHeight;

        public Camera(GraphicsDevice GraphicsDevice, GameWindow Window, int screenWidth, int screenHeight)
        {
            // Instantiate the camera
            viewportAdapter = new BoxingViewportAdapter(Window, GraphicsDevice, screenWidth, screenHeight);
            camera = new OrthographicCamera(viewportAdapter);
            viewMatrix = camera.GetViewMatrix();
            resolutionChange = false;
            this.screenWidth = screenWidth;
            this.screenHeight = screenHeight;
        }

        /*
         * Follows the player.
         * 
         * TO DO:
         * Fix issue where player moves off the bounds of the stage edge and springs back again.
         */
        public void FollowPlayer(
            GraphicsDevice GraphicsDevice, 
            GameWindow Window, 
            GraphicsDeviceManager graphics,
            GraphicsSettings graphicsSettings,
            Vector2 position)
        {
            // Check whether F4 was pressed and borderless toggled (TO DO: change to check this directly via ToggleBorderless later)
            if (graphicsSettings.ResolutionHasChanged())
            {
                // Get the new screen width and height
                screenWidth = graphics.PreferredBackBufferWidth;
                screenHeight = graphics.PreferredBackBufferHeight;

                // Reset the viewport adapter and camera
                viewportAdapter = new BoxingViewportAdapter(Window, GraphicsDevice, screenWidth, screenHeight);
                camera = new OrthographicCamera(viewportAdapter);

                graphicsSettings.ResolutionChangeFinished();

                // Debugging
                System.Diagnostics.Debug.WriteLine("Screen resolution changed: " + screenWidth + ", " + screenHeight);
            }

            camera.Move(camera.WorldToScreen(position.X - (screenWidth / 2), position.Y - (screenHeight / 2)));
        }

        /*
         * Get the camera.
         */
        public OrthographicCamera GetCamera()
        {
            return camera;
        }
    }
}