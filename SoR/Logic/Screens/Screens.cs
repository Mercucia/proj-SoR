﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SoR.Logic.Character;

namespace SoR.Logic.Screens
{
    /*
     * Tell GameLogic which aspects of the game it should currently be rendering and/or utilising
     * depending on the current game state.
     */
    public partial class Screens
    {
        private GameLogic gameLogic;

        public Screens(MainGame game, GraphicsDevice GraphicsDevice, GraphicsDeviceManager graphics)
        {
            gameLogic = new GameLogic(game, GraphicsDevice);
        }

        /*
         * Update the state of the game.
         */
        public void UpdateGameState(GameTime gameTime, MainGame game, GraphicsDevice GraphicsDevice, GraphicsDeviceManager graphics)
        {
            switch (gameLogic.CurrentMapString)
            {
                case "none":
                    if (gameLogic.CurrentInputScreen == "game")
                    {
                        gameLogic.UpdateWorld(gameTime, graphics);

                        foreach (var entity in gameLogic.Entities.Values)
                        {
                            if (gameLogic.Entities.TryGetValue("chara", out Entity chara))
                            {
                                if (chara.GetHitPoints() <= 98)
                                {
                                    gameLogic.FadingIn = true;
                                    if (gameLogic.CurtainUp)
                                    {
                                        gameLogic.Temple(game, GraphicsDevice);
                                    }
                                }
                            }
                        }
                    }
                    break;
                case "mainMenu":
                    gameLogic.GameMainMenu(game, GraphicsDevice, graphics);
                    gameLogic.CurrentMapString = "none";
                    break;
            }
            gameLogic.SaveLoadInput(game, gameTime, GraphicsDevice);
        }
    }
}