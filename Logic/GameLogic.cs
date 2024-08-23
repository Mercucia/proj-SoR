﻿using Microsoft.Xna.Framework;
using SoR.Logic.Entities;

namespace SoR.Logic
{
    /*
     * Placeholder class for handling game progression.
     */
    internal class GameLogic
    {
        private Entity player;
        private Entity pheasant;
        private Entity chara;
        private Entity slime;
        private Entity campfire;
        
        /*
         * Create player.
         */
        public Entity CreatePlayer(GraphicsDeviceManager _graphics)
        {
            player = new Player(_graphics);
            return player;
        }

        /*
         * Create pheasant.
         */
        public Entity CreatePheasant(GraphicsDeviceManager _graphics)
        {
            pheasant = new Pheasant(_graphics);
            return pheasant;
        }

        /*
         * Create chara.
         */
        public Entity CreateChara(GraphicsDeviceManager _graphics)
        {
            chara = new Chara(_graphics);
            return chara;
        }

        /*
         * Create slime.
         */
        public Entity CreateSlime(GraphicsDeviceManager _graphics)
        {
            slime = new Slime(_graphics);
            return slime;
        }

        /*
         * Create campfire.
         */
        public Entity CreateCampfire(GraphicsDeviceManager _graphics)
        {
            campfire = new Campfire(_graphics);
            return campfire;
        }

        /*
         * Placeholder game logic function
         */
        public void StartGame()
        {

        }
    }
}