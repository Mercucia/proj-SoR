﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SoR.Logic.Entities;
using System.Collections.Generic;

namespace SoR.Logic.Game
{
    /*
     * Placeholder class for handling game progression.
     */
    public class GameLogic
    {
        private Dictionary<string, Entity> entities;
        private EntityType entityType;
        private Vector2 centreScreen;

        /*
         * Enums for differentiating between entities.
         */
        enum EntityType
        {
            Player,
            Pheasant,
            Chara,
            Slime,
            Fire
        }

        /*
         * Constructor for initial game setup.
         */
        public GameLogic(GraphicsDeviceManager graphics, GraphicsDevice GraphicsDevice)
        {
            // Find the centre of the game window
            centreScreen = new Vector2(graphics.PreferredBackBufferWidth / 2,
                graphics.PreferredBackBufferHeight / 2);

            // Create a new dictionary for storing entities as values with string labels for keys
            entities = new Dictionary<string, Entity>();

            // Create the Player entity
            entityType = EntityType.Player;
            CreateEntity(graphics, GraphicsDevice);

            // Create the Slime entity
            entityType = EntityType.Slime;
            CreateEntity(graphics, GraphicsDevice);

            // Create the Chara entity
            entityType = EntityType.Chara;
            CreateEntity(graphics, GraphicsDevice);

            // Create the Pheasant entity
            entityType = EntityType.Pheasant;
            CreateEntity(graphics, GraphicsDevice);

            // Create the Campfire entity
            entityType = EntityType.Fire;
            CreateEntity(graphics, GraphicsDevice);
        }

        /*
         * Placeholder function for choosing which entity to create. Only use for permanent
         * entities - transient entities are fine being transient.
         */
        public void CreateEntity(GraphicsDeviceManager graphics, GraphicsDevice GraphicsDevice)
        {
            switch (entityType)
            {
                case EntityType.Player:
                    entities.Add("player", new Player(graphics, GraphicsDevice) { Name = "player" });
                    if (entities.TryGetValue("player", out Entity player))
                    {
                        player.GetScreenCentre(centreScreen);
                    }
                    break;
                case EntityType.Pheasant:
                    entities.Add("pheasant", new Pheasant(graphics, GraphicsDevice) { Name = "pheasant" });
                    if (entities.TryGetValue("pheasant", out Entity pheasant))
                    {
                        pheasant.GetScreenCentre(centreScreen);
                    }
                    break;
                case EntityType.Chara:
                    entities.Add("chara", new Chara(graphics, GraphicsDevice) { Name = "chara" });
                    if (entities.TryGetValue("chara", out Entity chara))
                    {
                        chara.GetScreenCentre(centreScreen);
                    }
                    break;
                case EntityType.Slime:
                    entities.Add("slime", new Slime(graphics, GraphicsDevice) { Name = "slime" });
                    if (entities.TryGetValue("slime", out Entity slime))
                    {
                        slime.GetScreenCentre(centreScreen);
                    }
                    break;
                case EntityType.Fire:
                    entities.Add("fire", new Campfire(graphics, GraphicsDevice) { Name = "fire" });
                    if (entities.TryGetValue("fire", out Entity fire))
                    {
                        fire.GetScreenCentre(centreScreen);
                    }
                    break;
            }
        }

        /*
         * Render Spine skeletons.
         */
        public void SpineRenderSkeleton(GraphicsDevice GraphicsDevice)
        {
            foreach (var entity in entities)
            {
                entity.Value.RenderSkeleton(GraphicsDevice); // Render each skeleton to the screen
            }
        }

        /*
         * Set up Spine animations and skeletons.
         */
        public void UpdateEntities(
            GameTime gameTime,
            KeyboardState keyState,
            KeyboardState lastKeyState,
            GraphicsDeviceManager graphics,
            GraphicsDevice GraphicsDevice)
        {
            if (entities.TryGetValue("player", out Entity playerChar))
            {
                if (playerChar is Player player)
                {
                    // Update position according to user input
                    player.UpdateEntityPosition(
                    gameTime,
                    keyState,
                    lastKeyState,
                    graphics,
                    GraphicsDevice,
                    player.GetAnimState(),
                    player.GetSkeleton());
                }
                else
                {
                    // Throw exception if playerChar is somehow not of the type Player
                    throw new System.InvalidOperationException("playerChar is not of type Player");
                }

                foreach (var entity in entities)
                {
                    // Update animations
                    entity.Value.UpdateEntityAnimations(gameTime);
                }

            }
        }
    }
}