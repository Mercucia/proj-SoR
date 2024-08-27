﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SoR.Logic.Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SoR.Logic.Game
{
    /*
     * Placeholder class for handling game progression.
     */
    public class GameLogic
    {
        private Dictionary<string, Entity> entities;
        private string[] presentEntities;
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

            // Create dictionary for storing entities as values with string labels for keys
            entities = new Dictionary<string, Entity>();

            // Create ArrayList to keep track of entities that are currently present
            presentEntities = new string[0];

            // Create the Player entity
            entityType = EntityType.Player;
            CreateEntity(graphics, GraphicsDevice);
            presentEntities.Append("player");

            // Create the Slime entity
            entityType = EntityType.Slime;
            CreateEntity(graphics, GraphicsDevice);
            presentEntities.Append("slime");

            // Create the Chara entity
            entityType = EntityType.Chara;
            CreateEntity(graphics, GraphicsDevice);
            presentEntities.Append("chara");

            // Create the Pheasant entity
            entityType = EntityType.Pheasant;
            CreateEntity(graphics, GraphicsDevice);
            presentEntities.Append("pheasant");

            // Create the Campfire entity
            entityType = EntityType.Fire;
            CreateEntity(graphics, GraphicsDevice);
            presentEntities.Append("fire");
        }

        /*
         * Choose entity to create and place it as a value in the entities dictionary with
         * a unique string identifier as a key.
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
         * Render Spine skeletons, and ensure entities that appear further down on the screen are in front
         * of those that are higher up.
         */
        public void SpineRenderSkeleton(GraphicsDevice GraphicsDevice)
        {
            // Sort entities by their y-axis position
            var sortByYAxis = entities.Values.OrderBy(entity => entity.PositionY);

            foreach (var entity in sortByYAxis)
            {
                entity.RenderSkeleton(GraphicsDevice); // Render each skeleton to the screen
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
            }

            foreach (var entity in entities)
            {
                // Update animations
                entity.Value.UpdateEntityAnimations(gameTime);
            }
        }

        /*
         * Placeholder function for dealing/taking damage.
         */
        public void Damage(Entity entity)
        {
            /*
            if (entities.TryGetValue("player", out Entity playerChar))
            {
                if (playerChar is Player player)
                {
                    If (entity.CollidesWith(player))
                    {
                        player.Battle(entity);
                    }
                }
                else
                {
                    // Throw exception if playerChar is somehow not of the type Player
                    throw new System.InvalidOperationException("playerChar is not of type Player");
                }
            }
             */
        }
    }
}