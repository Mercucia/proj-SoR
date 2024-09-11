﻿using Logic.Game.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SoR.Logic.Game;
using System;

namespace SoR
{
    /*
     * The main game class, through which all other code runs. This project utilises Spine and Monogame.
     */

    /**************************************************************************************************************************
     * Copyright (c) 2024, Katherine Town
     * 
     * Redistribution and use in source and binary forms, with or without modification, are permitted provided that the
     * following conditions are met:
     * 
     * 1. Redistributions of source code must retain the above copyright notice, this list of conditions and the following
     * disclaimer.
     * 
     * 2. Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following
     * disclaimer in the documentation and/or other materials provided with the distribution.
     * 
     * 3. Neither the name of the copyright holder nor the names of its contributors may be used to endorse or promote products
     * derived from this software without specific prior written permission.
     * 
     * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS “AS IS” AND ANY EXPRESS OR IMPLIED WARRANTIES,
     * INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
     * DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
     * SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
     * LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN
     * CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
     * SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
     * 
     * (The 3-Clause BSD License: https://opensource.org/license/BSD-3-Clause)
     **************************************************************************************************************************/

    public class MainGame : Game
    {
        private GraphicsDeviceManager graphics;
        private GameLogic gameLogic;
        private MainGame game;
        private GraphicsSettings graphicsSettings;
        private Settings settings;
        private bool resolutionChanging;

        /*
         * Constructor for the main game class. Initialises the graphics and mouse visibility.
         */
        public MainGame()
        {
            Content.RootDirectory = "Content";
            graphics = new GraphicsDeviceManager(this);
            resolutionChanging = false;
        }

        /*
         * Initialise the game.
         */
        protected override void Initialize()
        {
            game = this;

            settings = new Settings();

            graphicsSettings = new GraphicsSettings(game, graphics, Window, settings);
            graphicsSettings.InitialiseSettings(graphics, settings, Window);

            gameLogic = new GameLogic(graphics, GraphicsDevice, graphicsSettings, Window);

            base.Initialize();
        }

        /*
         * Load game content.
         */
        protected override void LoadContent()
        {
            gameLogic.LoadGameContent(graphics, GraphicsDevice, game);
            graphicsSettings.LoadSettings(game, Content);
        }

        /*
         * Unload graphics content.
         */
        protected override void UnloadContent()
        {
            graphicsSettings.UnloadSettings(Window, settings);

            base.UnloadContent();
        }

        /*
         * Update game components.
         */
        protected override void Update(GameTime gameTime)
        {
            // If the back button on the game pad or the escape key on the keyboard are pressed, exit the game
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            graphicsSettings.UpdateSettings(graphics, Window, settings);

            // Update player input and animations
            gameLogic.UpdateEntities(Window, gameTime, graphics, graphicsSettings, GraphicsDevice);

            base.Update(gameTime);
        }

        /*
         * Draw game components to the screen.
         */
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.DarkSeaGreen); // Clear the graphics buffer and set the window background colour to "dark sea green"

            graphicsSettings.DrawSettings(settings);
            
            gameLogic.Render(GraphicsDevice);

            base.Draw(gameTime);
        }

        void Window_ClientSizeChanged(object sender, EventArgs e)
        {
            resolutionChanging = !resolutionChanging;

            if (resolutionChanging)
            {
                float width = Window.ClientBounds.Width;
                float height = Window.ClientBounds.Height;

                float aspectRatio = (width / height);
                //System.Diagnostics.Debug.WriteLine();

                float x;
                float y;

                graphics.PreferredBackBufferWidth = Window.ClientBounds.Width;
                graphics.PreferredBackBufferHeight = Window.ClientBounds.Height;

                if (graphics.PreferredBackBufferWidth > 800 |
                    graphics.PreferredBackBufferHeight > 600)
                {
                    x = graphics.PreferredBackBufferWidth / 2;
                    y = graphics.PreferredBackBufferHeight / 2;
                }
                else
                {
                    graphics.PreferredBackBufferWidth = 800;
                    graphics.PreferredBackBufferHeight = 600;

                    x = 400;
                    y = 300;
                }

                gameLogic.RefocusCamera(GraphicsDevice, Window, (int)x, (int)y);

                graphics.ApplyChanges();

                resolutionChanging = false;
            }
        }
    }
}