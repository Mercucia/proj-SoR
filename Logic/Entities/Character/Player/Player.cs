﻿using Logic.Game.GameMap.TiledScenery;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SoR;
using SoR.Logic.Entities;
using SoR.Logic.Input;
using Spine;
using System.Collections.Generic;
using System.Windows.Input;

namespace Logic.Entities.Character.Player
{
    /*
     * Stores information unique to Player.
     */
    public class Player : Entity
    {
        private string skin;
        private KeyboardState lastKeyState;

        public Player(GraphicsDevice GraphicsDevice)
        {
            // The possible animations to play as a string and the method to use for playing them as an int
            animations = new Dictionary<string, int>()
            {
                { "idlebattle", 1 },
                { "runup", 3 },
                { "rundown", 3 },
                { "runleft", 3 },
                { "runright", 3 }
            };

            // Load texture atlas and attachment loader
            atlas = new Atlas(Globals.GetPath("Content\\SoR Resources\\Entities\\Player\\Char sprites.atlas"), new XnaTextureLoader(GraphicsDevice));
            atlasAttachmentLoader = new AtlasAttachmentLoader(atlas);
            json = new SkeletonJson(atlasAttachmentLoader);

            // Initialise skeleton json
            skeletonData = json.ReadSkeletonData(Globals.GetPath("Content\\SoR Resources\\Entities\\Player\\skeleton.json"));
            skeleton = new Skeleton(skeletonData);

            // Set the skin
            skeleton.SetSkin(skeletonData.FindSkin("solarknight-0"));
            skin = "solarknight-0";

            // Setup animation
            animStateData = new AnimationStateData(skeleton.Data);
            animState = new AnimationState(animStateData);
            animState.Apply(skeleton);
            animStateData.DefaultMix = 0.1f;

            // Set the "fidle" animation on track 1 and leave it looping forever
            trackEntry = animState.SetAnimation(0, "idlebattle", true);

            // Create hitbox
            slot = skeleton.FindSlot("hitbox");
            hitboxAttachment = skeleton.GetAttachment("hitbox", "hitbox");
            slot.Attachment = hitboxAttachment;
            skeleton.SetAttachment("hitbox", "hitbox");

            hitbox = new SkeletonBounds();
            hitbox.Update(skeleton, true);

            movement = new Movement(); // Environmental collision handling

            Speed = 200f; // Set the entity's travel speed

            hitpoints = 100; // Set the starting number of hitpoints

            countDistance = 0;

            Height = 1;
        }

        /*
         * Placeholder function for handling battles.
         */
        public void Battle(Entity entity)
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

        /*
         * If the player pressed space, switch to the next skin.
         */
        public void CheckSwitchSkin()
        {
            if (movement.SkinHasChanged())
            {
                switch (skin)
                {
                    case "solarknight-0":
                        skeleton.SetSkin(skeletonData.FindSkin("lunarknight-0"));
                        skin = "lunarknight-0";
                        break;
                    case "lunarknight-0":
                        skeleton.SetSkin(skeletonData.FindSkin("knight-0"));
                        skin = "knight-0";
                        break;
                    case "knight-0":
                        skeleton.SetSkin(skeletonData.FindSkin("solarknight-0"));
                        skin = "solarknight-0";
                        break;
                }
            }
        }

        /*
         * Update entity position.
         */
        public override void UpdatePosition(GameTime gameTime, GraphicsDeviceManager graphics, List<Rectangle> WalkableArea)
        {
            prevPosition = position;

            GetMoved(gameTime);

            // Process joypad inputs
            movement.ProcessJoypadInputs(gameTime, Speed);
            movement.CheckMovement(gameTime, Speed, position, WalkableArea, this);
            ChangeAnimation(movement.AnimateMovement());

            // Set the new position
            position = movement.UpdatePosition();

            if (movement.IsTraversable())
            {
                lastKeyState = movement.GetLastKeyState();
            }

            if (!movement.IsTraversable())
            {
                if (lastKeyState.IsKeyDown(Keys.Left) || lastKeyState.IsKeyDown(Keys.A)) // Right
                {
                    ThrownBack(gameTime, position.X - 1, position.Y, 1);
                }
                if (lastKeyState.IsKeyDown(Keys.Right) || lastKeyState.IsKeyDown(Keys.D)) // Left
                {
                    ThrownBack(gameTime, position.X + 1, position.Y, 1);
                }
                if (lastKeyState.IsKeyDown(Keys.Up) || lastKeyState.IsKeyDown(Keys.W)) // Down
                {
                    ThrownBack(gameTime, position.X, position.Y - 1, 1);
                }
                if (lastKeyState.IsKeyDown(Keys.Down) || lastKeyState.IsKeyDown(Keys.S)) // Up
                {
                    ThrownBack(gameTime, position.X, position.Y + 1, 1);
                }
            }
        }

        /*
         * Update the skeleton position, skin and animation state.
         */
        public override void UpdateAnimations(GameTime gameTime)
        {
            base.UpdateAnimations(gameTime);

            // Check whether to change the skin
            CheckSwitchSkin();
        }
    }
}