﻿using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using SoR;

namespace Logic.Game.Screens
{
    public partial class Screens
    {
        /*
         * Currently the same every time the game is started, but will be updated later when file saving implemented.
         */
        public void LoadGameState(MainGame game, GraphicsDevice GraphicsDevice)
        {
            gameLogic.Village(game, GraphicsDevice, true);
        }

        /*
         * Update the resolution after a screen size change.
         */
        public void UpdateResolution(GameWindow Window, int screenWidth, int screenHeight)
        {
            gameLogic.UpdateViewportGraphics(Window, screenWidth, screenHeight);
        }

        /*
         * Draw the current state of the game to the screen.
         */
        public void DrawGameState(GraphicsDevice GraphicsDevice)
        {
            gameLogic.Render(GraphicsDevice);
        }
    }
}