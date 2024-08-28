﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SoR.Logic.Input;
using Spine;

namespace SoR.Logic.Entities
{
    /*
     * Stores information unique to the player entity.
     */
    public class Player : Entity
    {
        private PlayerInput playerInput;
        private string skin;

        public Player(GraphicsDeviceManager graphics, GraphicsDevice GraphicsDevice)
        {
            // Load texture atlas and attachment loader
            atlas = new Atlas("F:\\MonoGame\\SoR\\SoR\\Content\\Entities\\Player\\Char sprites.atlas", new XnaTextureLoader(GraphicsDevice));
            //atlas = new Atlas("D:\\GitHub projects\\Proj-SoR\\Content\\Entities\\Player\\Char sprites.atlas", new XnaTextureLoader(GraphicsDevice));
            atlasAttachmentLoader = new AtlasAttachmentLoader(atlas);
            json = new SkeletonJson(atlasAttachmentLoader);

            // Initialise skeleton json
            skeletonData = json.ReadSkeletonData("F:\\MonoGame\\SoR\\SoR\\Content\\Entities\\Player\\skeleton.json");
            //skeletonData = json.ReadSkeletonData("D:\\GitHub projects\\Proj-SoR\\Content\\Entities\\Player\\skeleton.json");
            skeleton = new Skeleton(skeletonData);

            // Set the skin
            skeleton.SetSkin(skeletonData.FindSkin("solarknight-0"));
            skin = "solarknight-0";

            // Setup animation
            animStateData = new AnimationStateData(skeleton.Data);
            animState = new AnimationState(animStateData);
            animState.Apply(skeleton);

            // Set the "fidle" animation on track 1 and leave it looping forever
            animState.SetAnimation(0, "idlebattle", true);

            // Create hitbox
            slot = skeleton.FindSlot("hitbox");
            hitboxAttachment = skeleton.GetAttachment("hitbox", "hitbox");
            slot.Attachment = hitboxAttachment;
            skeleton.SetAttachment("hitbox", "hitbox");

            // Initialise skeleton renderer with premultiplied alpha
            skeletonRenderer = new SkeletonRenderer(GraphicsDevice);
            skeletonRenderer.PremultipliedAlpha = true;

            playerInput = new PlayerInput(); // Instantiate the keyboard input

            // Set the current position on the screen
            position = new Vector2(graphics.PreferredBackBufferWidth / 2,
                graphics.PreferredBackBufferHeight / 2);

            positionX = position.X; // Set the x-axis position
            positionY = position.Y; // Set the y-axis position

            Speed = 200f; // Set the entity's travel speed

            hitpoints = 100; // Set the starting number of hitpoints
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
         * Check for collision with other entities.
         */
        public override bool CollidesWith(Entity entity)
        {
            entity.UpdateHitbox(new SkeletonBounds());
            entity.GetHitbox().Update(entity.GetSkeleton(), true);

            hitbox = new SkeletonBounds();
            hitbox.Update(skeleton, true);

            if (hitbox.AabbIntersectsSkeleton(entity.GetHitbox()))
            {
                return true;
            }

            /*// Pull hitbox apart and put it back together again
            // Can't remember right now why this was necessary, think it was to do with the shield
            foreach (Polygon polygon in entityHitbox.Polygons)
            {
                ArrayList vertices = new ArrayList();

                for (int i = 0; i < polygon.Vertices.Length; i = i + 2)
                {
                    // Add each vertex's x,y coordinate pair to the new vertices array
                    Vector2 point = new Vector2(polygon.Vertices[i], polygon.Vertices[i + 1]);
                    vertices.Add(point);
                }

                for (int i = 0; i < vertices.Count; i++)
                {
                    // Update the hitbox with the new x,y coordinates
                    Vector2 pointOne = (Vector2)vertices[i];
                    Vector2 pointTwo = new Vector2();

                    if (i == 0)
                    {
                        pointTwo = (Vector2)vertices[vertices.Count - 1];
                    }
                    else
                    {
                        pointTwo = (Vector2)vertices[i - 1];
                    }

                    if (hitbox.IntersectsSegment(pointOne.X, pointOne.Y, pointTwo.X, pointTwo.Y) != null)
                    {
                        return true;
                    }
                }
            }*/

            return false;
        }

        /*
         * Update the hitbox after a collision.
         */
        public override void UpdateHitbox(SkeletonBounds updatedHitbox)
        {
            hitbox = updatedHitbox;
        }

        /*
         * If the player pressed space, switch to the next skin.
         */
        public void CheckSwitchSkin()
        {
            if (playerInput.SkinHasChanged())
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
         * Get the animation state.
         */
        public override AnimationState GetAnimState()
        {
            return animState;
        }

        /*
         * Get the skeleton.
         */
        public override Skeleton GetSkeleton()
        {
            return skeleton;
        }

        /*
         * Get the hitbox.
         */
        public override SkeletonBounds GetHitbox()
        {
            return hitbox;
        }

        /*
         * Update entity position according to player input.
         */
        public void UpdateEntityPosition(
            GameTime gameTime,
            GraphicsDeviceManager graphics,
            GraphicsDevice GraphicsDevice,
            AnimationState animState,
            Skeleton skeleton)
        {
            prevPositionX = positionX;
            prevPositionY = positionY;

            // Pass the speed, position and animation state to PlayerInput for keyboard input processing
            playerInput.ProcessKeyboardInputs(gameTime,
                animState,
                Speed,
                positionX,
                positionY);

            // Pass the speed to PlayerInput for joypad input processing
            playerInput.ProcessJoypadInputs(gameTime, Speed);

            // Set the new position according to player input
            positionX = playerInput.UpdatePositionX();
            positionY = playerInput.UpdatePositionY();

            // Handle player collision
            playerInput.EnvironCollision(graphics,
                GraphicsDevice,
                positionX,
                positionY);

            // Set the new position according to player input
            positionX = playerInput.UpdatePositionX();
            positionY = playerInput.UpdatePositionY();
        }

        /*
         * Handle entity collision.
         * 
         * TO DO:
         * Player should still be able to move perpendicular to hitbox edge when in collision.
         */
        public void EntityCollision(
            GameTime gameTime,
            SkeletonBounds playerBox,
            SkeletonBounds entityBox)
        {
            if (playerBox.MaxX > entityBox.MinX
            | playerBox.MinX < entityBox.MaxX
            | playerBox.MinY > entityBox.Height
            | playerBox.MaxY < entityBox.Height)
            {
                positionX = prevPositionX;
                positionY = prevPositionY;
            }
        }

        /*
         * Update the skeleton position, skin and animation state.
         */
        public override void UpdateEntityAnimations(GameTime gameTime)
        {
            // Check whether to change the skin
            CheckSwitchSkin();

            // Update the animation state and apply animations to skeletons
            skeleton.X = positionX;
            skeleton.Y = positionY;

            hitbox.Update(skeleton, true);
            animState.Update((float)gameTime.ElapsedGameTime.TotalSeconds);
            skeleton.Update((float)gameTime.ElapsedGameTime.TotalSeconds);
            animState.Apply(skeleton);

            // Update skeletal transformations
            skeleton.UpdateWorldTransform(Skeleton.Physics.Update);
        }

        /*
         * Render the current skeleton to the screen.
         */
        public override void RenderSkeleton(GraphicsDevice GraphicsDevice)
        {
            // Create the skeleton renderer projection matrix
            ((BasicEffect)skeletonRenderer.Effect).Projection = Matrix.CreateOrthographicOffCenter(
            0,
                GraphicsDevice.Viewport.Width,
                GraphicsDevice.Viewport.Height,
                0, 1, 0);

            // Draw skeletons
            skeletonRenderer.Begin();
            skeletonRenderer.Draw(skeleton);
            skeletonRenderer.End();

            // Set the text above the character to show the MaxX, MaxY, MinX, MinY, positionX and positionY
            showMaxX = hitbox.MaxX.ToString();
            showMaxY = hitbox.MaxY.ToString();
            showMinX = hitbox.MinX.ToString();
            showMinY = hitbox.MinY.ToString();
            showPositionX = positionX.ToString();
            showPositionY = positionY.ToString();
        }

        /*
         * Draw text to the screen (debugging).
         */
        public override void DrawText(SpriteBatch spriteBatch, SpriteFont font)
        {
            spriteBatch.Begin();
            spriteBatch.DrawString(
                font,
                "\npositionX: " + showPositionX + ", positionY: " + showPositionY,
                new Vector2(positionX - 150, positionY - hitbox.Height * 7F),
                Color.BlueViolet);
            spriteBatch.End();
        }

        /* 
         * Get the centre of the screen.
         */
        public override void GetScreenCentre(Vector2 centreScreen)
        {
            position = centreScreen;

            position = new Vector2(position.X - 270, position.Y - 150);

            positionX = position.X; // Set the x-axis position
            positionY = position.Y; // Set the y-axis position
        }

        /*
         * Get the current x-axis position.
         */
        public override float GetPositionX()
        {

            return positionX;
        }

        /*
         * Get the current y-axis position.
         */
        public override float GetPositionY()
        {
            return positionY;
        }

        /*
         * Set the new x-axis position.
         */
        public void SetPositionX(float newPositionX)
        {

            positionX = newPositionX;
        }

        /*
         * Set the new y-axis position.
         */
        public void SetPositionY(float newPositionY)
        {
            positionY = newPositionY;
        }
    }
}