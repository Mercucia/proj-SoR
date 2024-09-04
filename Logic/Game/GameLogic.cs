﻿using Logic.Entities.Character.Player;
using Logic.Game;
using Logic.Locations;
using Logic.Locations.Interactables;
using Logic.Entities.Character.Mobs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SoR.Logic.Entities;
using System.Collections.Generic;
using System.Linq;

namespace SoR.Logic.Game
{
    /*
     * Placeholder class for handling game progression.
     */
    public class GameLogic
    {
        private EntityType entityType;
        private SceneryType sceneryType;
        private Dictionary<string, Entity> entities;
        private Dictionary<string, Scenery> scenery;
        private Camera camera;
        private SpriteBatch spriteBatch;
        private SpriteFont font;

        /*
         * Enums for differentiating between entities.
         */
        enum EntityType
        {
            Player,
            Pheasant,
            Chara,
            Slime
        }

        /*
         * Enums for differentiating between environmental ojects.
         */
        enum SceneryType
        {
            Campfire,
            Grass
        }

        /*
         * Constructor for initial game setup.
         */
        public GameLogic()
        {
            // Create dictionary for storing entities as values with string labels for keys
            entities = new Dictionary<string, Entity>();

            // Create dictionary for storing entities as values with string labels for keys
            scenery = new Dictionary<string, Scenery>();
        }

        /*
         * 
         */
        public void InitialiseCamera(Viewport viewport)
        {
            camera = new Camera(viewport);
        }

         /*
          * 
          */
        public void LoadGameContent(GraphicsDeviceManager graphics, GraphicsDevice GraphicsDevice, MainGame game)
        {
            Vector2 centreScreen = new Vector2(graphics.PreferredBackBufferWidth / 2, graphics.PreferredBackBufferHeight / 2);

            // Initialise SpriteBatch
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // Font used for drawing text
            font = game.Content.Load<SpriteFont>("Fonts/File");

            // Create the Player entity
            entityType = EntityType.Player;
            CreateEntity(graphics, GraphicsDevice, centreScreen);

            // Create the Slime entity
            entityType = EntityType.Slime;
            CreateEntity(graphics, GraphicsDevice, centreScreen);

            // Create the Chara entity
            entityType = EntityType.Chara;
            CreateEntity(graphics, GraphicsDevice, centreScreen);

            // Create the Pheasant entity
            entityType = EntityType.Pheasant;
            CreateEntity(graphics, GraphicsDevice, centreScreen);

            // Create the Campfire object
            sceneryType = SceneryType.Campfire;
            CreateObject(graphics, GraphicsDevice);

            // Create the Campfire object
            sceneryType = SceneryType.Grass;
            CreateObject(graphics, GraphicsDevice);
        }

        /*
         * Choose entity to create and place it as a value in the entities dictionary with
         * a unique string identifier as a key.
         */
        public void CreateEntity(GraphicsDeviceManager graphics, GraphicsDevice GraphicsDevice, Vector2 centreScreen)
        {
            switch (entityType)
            {
                case EntityType.Player:
                    entities.Add("player", new Player(graphics, GraphicsDevice) { Render = true });
                    if (entities.TryGetValue("player", out Entity player))
                    {
                        player.SetStartPosition(graphics, centreScreen);
                    }
                    break;
                case EntityType.Pheasant:
                    entities.Add("pheasant", new Pheasant(graphics, GraphicsDevice) { Render = true });
                    if (entities.TryGetValue("pheasant", out Entity pheasant))
                    {
                        pheasant.SetStartPosition(graphics, centreScreen);
                    }
                    break;
                case EntityType.Chara:
                    entities.Add("chara", new Chara(graphics, GraphicsDevice) { Render = true });
                    if (entities.TryGetValue("chara", out Entity chara))
                    {
                        chara.SetStartPosition(graphics, centreScreen);
                    }
                    break;
                case EntityType.Slime:
                    entities.Add("slime", new Slime(graphics, GraphicsDevice) { Render = true });
                    if (entities.TryGetValue("slime", out Entity slime))
                    {
                        slime.SetStartPosition(graphics, centreScreen);
                    }
                    break;
            }
        }

        /*
         * Choose environmental object to create and place it as a value in the objects dictionary with
         * a unique string identifier as a key.
         */
        public void CreateObject(GraphicsDeviceManager graphics, GraphicsDevice GraphicsDevice)
        {
            switch (sceneryType)
            {
                case SceneryType.Campfire:
                    scenery.Add("campfire", new Campfire(graphics, GraphicsDevice) { Render = true });
                    if (scenery.TryGetValue("campfire", out Scenery campfire))
                    {
                        campfire.SetStartPosition();
                    }
                    break;
            }
            switch (sceneryType)
            {
                case SceneryType.Grass:
                    scenery.Add("grass", new Grass(graphics, GraphicsDevice) { Render = true });
                    if (scenery.TryGetValue("grass", out Scenery grass))
                    {
                        grass.SetStartPosition();
                    }
                    break;
            }
        }

        /*
         * Updates the player according to camera position.
         */
        public void UpdatePlayerPosition(Entity player)
        {
            player.SetPosition(camera.GetCameraPositionX(), camera.GetCameraPositionY());
        }

        /*
         * Update Spine animations and skeletons.
         */
        public void UpdateEntities(
            GameTime gameTime,
            GraphicsDeviceManager graphics,
            GraphicsDevice GraphicsDevice,
            GraphicsSettings graphicsSettings)
        {
            foreach (var scenery in scenery.Values)
            {
                if (scenery.Render)
                {
                    // Update animations
                    scenery.UpdateSceneryAnimations(gameTime);
                }
            }

            foreach (var entity in entities.Values)
            {
                if (entity.Render)
                {
                    entity.Movement(gameTime, graphics);

                    // Update position according to user input
                    entity.UpdatePosition(
                    gameTime,
                    graphics,
                    GraphicsDevice);

                    if (entities.TryGetValue("player", out Entity playerChar))
                    {
                        if (playerChar is Player player)
                        {
                            camera.UpdateCamera(player);

                            if (entity != player & player.CollidesWith(entity))
                            {
                                player.ChangeAnimation("collision");

                                entity.StopMoving();
                            }
                            else if (!entity.IsMoving())
                            {
                                entity.StartMoving();
                            }
                        }
                        else
                        {
                            // Throw exception if playerChar is somehow not of the type Player
                            throw new System.InvalidOperationException("playerChar is not of type Player");
                        }
                    }

                    // Update animations
                    entity.UpdateEntityAnimations(gameTime, camera.GetCameraPositionX(), camera.GetCameraPositionY());
                }
            }
        }

        /*
         * Render Spine objects in order of y-axis position.
         */
        public void Render(GraphicsDevice GraphicsDevice)
        {
            var sortSceneryByYAxis = scenery.Values.OrderBy(scenery => scenery.GetSceneryHitbox().MaxY);
            var sortEntitiesByYAxis = entities.Values.OrderBy(entity => entity.GetEntityHitbox().MaxY);

            foreach (var scenery in sortSceneryByYAxis)
            {
                if (scenery.Render)
                {
                    scenery.RenderScenery(GraphicsDevice);
                    scenery.DrawSceneryText(spriteBatch, font);
                }
            }

            foreach (var entity in sortEntitiesByYAxis)
            {
                if (entity.Render)
                {
                    entity.RenderEntity(GraphicsDevice);
                    entity.DrawEntityText(spriteBatch, font);
                }
            }
        }
    }
}