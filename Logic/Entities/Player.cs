﻿namespace SoR.Logic.Entities
{
    internal class Player : Entity
    {
        /*
         * Get the entity Atlas path.
         */
        public string GetAtlas()
        {
            return "F:\\MonoGame\\SoR\\SoR\\Content\\Entities\\Player\\Char sprites.atlas";
        }

        /*
         * Get the entity json path.
         */
        public string GetJson()
        {
            return "F:\\MonoGame\\SoR\\SoR\\Content\\Entities\\Player\\skeleton.json";
        }

        /*
         * Get the entity starting skin.
         */
        public string GetSkin()
        {
            return "solarknight-0";
        }

        /*
         * Get the entity starting animation.
         */
        public string GetStartingAnim()
        {
            return "idlebattle";
        }
    }
}