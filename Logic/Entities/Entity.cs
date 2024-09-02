﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SoR.Logic.Input;
using Spine;
using System;

namespace SoR.Logic.Entities
{
    /*
     * All other entities are currently based on this class to reduce code repetition.
     */
    public abstract class Entity
    {
        protected Atlas atlas;
        protected AtlasAttachmentLoader atlasAttachmentLoader;
        protected SkeletonJson json;
        protected SkeletonData skeletonData;
        protected SkeletonRenderer skeletonRenderer;
        protected Skeleton skeleton;
        protected AnimationStateData animStateData;
        protected AnimationState animState;
        protected Attachment hitboxAttachment;
        protected SkeletonBounds hitbox;
        protected Slot slot;
        protected TrackEntry trackEntry;
        protected InputMovement movement;
        protected Random random;
        protected Vector2 position;
        protected Vector2 movementDirection;
        protected float prevPositionX;
        protected float prevPositionY;
        protected int hitpoints;
        protected string prevTrigger;
        protected string animOne;
        protected string animTwo;
        protected float newDirectionTime;
        protected float sinceLastChange;
        protected bool inMotion;

        public float Speed { get; set; }
        public string Name { get; set; }
        public bool Render { get; set; }

        /*
         * Placeholder function for dealing damage.
         */
        public abstract int Damage(Entity entity);

        /*
         * If something changes to trigger a new animation, apply the animation.
         * If the animation is already applied, do nothing.
         */
        public abstract void ChangeAnimation(string trigger);

        /* 
         * Get the centre of the screen.
         */
        public abstract void SetStartPosition(Vector2 centreScreen);

        /*
         * Check if moving.
         */
        public bool IsMoving()
        {
            return inMotion;
        }

        /*
         * Start moving and switch to running animation.
         */
        public void StartMoving()
        {
            inMotion = true;
            ChangeAnimation("move");
        }

        /*
         * Stop moving.
         */
        public void StopMoving()
        {
            inMotion = false;
            ChangeAnimation("collision"); // TO DO: Fix - see collision
            position.X = prevPositionX;
            position.Y = prevPositionY;
        }

        /*
         * Choose a method for playing the animation according to Player.ChangeAnimation(eventTrigger)
         * animType.
         */
        public void React(string reaction, int animType)
        {
            if (reaction != "none")
            {
                if (animType == 1)
                {
                    animState.AddAnimation(0, animOne, true, -trackEntry.TrackComplete);
                }
                if (animType == 2)
                {
                    animState.SetAnimation(0, animOne, false);
                    trackEntry = animState.AddAnimation(0, animTwo, true, 0);
                }
                if (animType == 3)
                {
                    animState.AddAnimation(0, animOne, true, -trackEntry.TrackTime);
                }
            }
        }

        /*
         * Update entity position.
         */
        public virtual void UpdatePosition(GameTime gameTime, GraphicsDeviceManager graphics, GraphicsDevice GraphicsDevice)
        {
            // Handle environmental collision
            if (movement.EnvironCollision(
                graphics,
                GraphicsDevice,
                GetHitbox(),
                position.X,
                position.Y))
            {
                NewDirection(movement.TurnAround());
            }

            // Set the new position
            position = new Vector2(movement.UpdatePositionX(), movement.UpdatePositionY());
        }

        /*
         * Choose a new direction to face.
         */
        public void NewDirection(int direction)
        {
            switch (direction)
            {
                case 1:
                    movementDirection = new Vector2(-1, 0); // Left
                    ChangeAnimation("turnleft");
                    break;
                case 2:
                    movementDirection = new Vector2(1, 0); // Right
                    ChangeAnimation("turnright");
                    break;
                case 3:
                    movementDirection = new Vector2(0, -1); // Up
                    break;
                case 4:
                    movementDirection = new Vector2(0, 1); // Down
                    break;
            }
        }

        /*
         * Move to new position.
         */
        public virtual void Movement(GameTime gameTime)
        {
            prevPositionX = position.X;
            prevPositionY = position.Y;

            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            float newSpeed = Speed * deltaTime;

            sinceLastChange += deltaTime;

            if (IsMoving())
            {
                if (sinceLastChange >= newDirectionTime)
                {
                    int direction = random.Next(4);
                    NewDirection(direction);
                    newDirectionTime = (float)random.NextDouble() * 3f + 0.33f;
                    sinceLastChange = 0;
                }

                position += movementDirection * newSpeed;
            }
        }

        /*
         * Check for collision with other entities.
         */
        public bool CollidesWith(Entity entity)
        {
            entity.UpdateHitbox(new SkeletonBounds());
            entity.GetHitbox().Update(entity.GetSkeleton(), true);

            hitbox = new SkeletonBounds();
            hitbox.Update(skeleton, true);

            if (hitbox.AabbIntersectsSkeleton(entity.GetHitbox()))
            {
                return true;
            }

            return false;
        }

        /*
         * Render the current skeleton to the screen.
         */
        public void RenderSkeleton(GraphicsDevice GraphicsDevice)
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
        }

        /*
         * Update the entity position, animation state and skeleton.
         */
        public virtual void UpdateEntityAnimations(GameTime gameTime)
        {
            // Update the animation state and apply animations to skeletons
            skeleton.X = position.X;
            skeleton.Y = position.Y;

            hitbox.Update(skeleton, true);
            animState.Update((float)gameTime.ElapsedGameTime.TotalSeconds);
            skeleton.Update((float)gameTime.ElapsedGameTime.TotalSeconds);
            animState.Apply(skeleton);

            // Update skeletal transformations
            skeleton.UpdateWorldTransform(Skeleton.Physics.Update);
        }

        /*
         * Update the hitbox after a collision.
         */
        public void UpdateHitbox(SkeletonBounds updatedHitbox)
        {
            hitbox = updatedHitbox;
        }

        /*
         * Draw text to the screen.
         */
        public void DrawText(SpriteBatch spriteBatch, SpriteFont font)
        {
            spriteBatch.Begin();
            spriteBatch.DrawString(
                font,
                "animOne: " + animOne + " animTwo: " + animTwo +
                "\n prevTrigger: " + prevTrigger,
                new Vector2(position.X - 50, position.Y + hitbox.Height / 2),
                Color.BlueViolet);
            spriteBatch.End();
        }

        /*
         * Get the skeleton.
         */
        public Skeleton GetSkeleton()
        {
            return skeleton;
        }

        /*
         * Get the hitbox.
         */
        public SkeletonBounds GetHitbox()
        {
            return hitbox;
        }
    }
}