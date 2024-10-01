﻿using Logic.Entities.Character;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SoR;

namespace Logic.Game.Screens
{
    /*
     * Placeholder class for game stages. Take components from GameLogic to set the initial state of the game, then manage progression.
     */
    public partial class Screens
    {
        private GameLogic gameLogic;

        public Screens(GraphicsDevice GraphicsDevice, GameWindow Window)
        {
            gameLogic = new GameLogic(GraphicsDevice, Window);
        }

        /*
         * Update the state of the game.
         */
        public void UpdateGameState(GameTime gameTime, MainGame game, GraphicsDevice GraphicsDevice, GraphicsDeviceManager graphics)
        {
            switch (gameLogic.CurrentMapString)
            {
                case "none":
                    gameLogic.UpdateWorld(gameTime, graphics);

                    foreach (var entity in gameLogic.Entities.Values)
                    {
                        if (gameLogic.Entities.TryGetValue("chara", out Entity chara))
                        {
                            if (chara.GetHitPoints() <= 98)
                            {
                                gameLogic.Temple(game, GraphicsDevice, true);
                            }
                        }
                    }
                    break;
                case "village":
                    gameLogic.Village(game, GraphicsDevice, true);
                    gameLogic.CurrentMapString = "none";
                    break;
                case "temple":
                    gameLogic.Temple(game, GraphicsDevice, true);
                    gameLogic.CurrentMapString = "none";
                    break;
            }
            gameLogic.SaveLoadGameData(game, GraphicsDevice);
        }
    }
}