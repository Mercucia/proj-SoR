﻿using Logic.Game.GameMap.TiledScenery;
using Microsoft.Xna.Framework;
using SoR.Logic.Input;
using Spine;
using System;
using System.Collections.Generic;

namespace SoR.Logic.Entities
{
    /*
     * Spine Runtimes License
     */
    /**************************************************************************************************************************
     * Copyright (c) 2013-2024, Esoteric Software LLC
     * 
     * Integration of the Spine Runtimes into software or otherwise creating derivative works of the Spine Runtimes is
     * permitted under the terms and conditions of Section 2 of the Spine Editor License Agreement:
     * http://esotericsoftware.com/spine-editor-license
     * 
     * Otherwise, it is permitted to integrate the Spine Runtimes into software or otherwise create derivative works of the
     * Spine Runtimes (collectively, "Products"), provided that each user of the Products must obtain their own Spine Editor
     * license and redistribution of the Products in any form must include this license and copyright notice.
     * 
     * THE SPINE RUNTIMES ARE PROVIDED BY ESOTERIC SOFTWARE LLC "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT
     * NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO
     * EVENT SHALL ESOTERIC SOFTWARE LLC BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
     * DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES, BUSINESS INTERRUPTION, OR LOSS OF
     * USE, DATA, OR PROFITS) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
     * (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THE SPINE RUNTIMES, EVEN IF ADVISED OF THE
     * POSSIBILITY OF SUCH DAMAGE.
     **************************************************************************************************************************/

    /*
     * Common functions and fields for player and non-player characters.
     */
    public abstract class Entity
    {
        protected Dictionary<string, int> animations;
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
        protected Random random;
        protected Movement movement;
        protected int countDistance;
        protected Vector2 position;
        protected Vector2 movementDirection;
        protected Vector2 pushedDirection;
        protected Vector2 prevPosition;
        protected int hitpoints;
        protected string prevTrigger;
        protected string animOne;
        protected string animTwo;
        protected float newDirectionTime;
        protected float sinceLastChange;
        protected bool inMotion;

        public Rectangle rect; // debugging

        public int Height { get; protected set; }
        public string Name { get; set; }
        public float Speed { get; set; }

        /*
         * Placeholder function for dealing damage.
         */
        public void TakeDamage(int damage)
        {
            hitpoints -= damage;
        }

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
            ChangeAnimation("collision");
            position = prevPosition;
        }

        /*
         * If something changes to trigger a new animation, apply the animation.
         * If the animation is already applied, do nothing.
         */
        public void ChangeAnimation(string eventTrigger)
        {
            string reaction = "none"; // Default to "none" if there will be no animation change

            if (prevTrigger != eventTrigger)
            {
                foreach (string animation in animations.Keys)
                {
                    if (eventTrigger == animation)
                    {
                        prevTrigger = animOne = reaction = animation;

                        React(reaction, animations[animation]);
                    }
                }
            }
        }

        /*
         * Choose a method for playing the animation according to Player.ChangeAnimation(eventTrigger)
         * animType.
         * 
         * 1 = rapidly transition to next animation
         * 2 = set new animation then queue the next
         * 3 = start next animation on the same frame the previous animation finished on
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
         * Choose a new direction to face.
         */
        public void NewDirection(int direction)
        {
            switch (direction)
            {
                case 1:
                    movementDirection = new Vector2(-1, 0); // Left
                    ChangeAnimation("turnleft");
                    skeleton.ScaleX = 1;
                    break;
                case 2:
                    movementDirection = new Vector2(1, 0); // Right
                    ChangeAnimation("turnright");
                    skeleton.ScaleX = -1;
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
         * If the entity is thrown back from something, move away from that thing.
         */
        public void ThrownBack(GameTime gameTime, float thrownFromX, float thrownFromY, int throwDistance)
        {
            hitbox = new SkeletonBounds();
            hitbox.Update(skeleton, true);

            countDistance = throwDistance;
            while (countDistance < throwDistance)
            {
                countDistance++;
            }

            float newSpeed = (float)(Speed * 1.5) * (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (position.X > thrownFromX) // Right
            {
                pushedDirection.X += 1;
            }
            else if (position.X < thrownFromX) // Left
            {
                pushedDirection.X -= 1;
            }
            if (position.Y > thrownFromY) // Down
            {
                pushedDirection.Y += 1;
            }
            else if (position.Y < thrownFromY) // Up
            {
                pushedDirection.Y -= 1;
            }
        }

        /*
         * Get moved automatically.
         */
        public void GetMoved(GameTime gameTime)
        {
            if (countDistance > 0)
            {
                float newSpeed = Speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                position += pushedDirection * newSpeed;

                if (countDistance == 1)
                {
                    pushedDirection = Vector2.Zero;
                }

                countDistance--;
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
         * Define what happens on collision with an entity.
         */
        public virtual void Collision(Entity entity, GameTime gameTime)
        {
            entity.TakeDamage(1);
            entity.ThrownBack(gameTime, position.X, position.Y, 5);
        }

        /*
         * Update entity position.
         */
        public virtual void UpdatePosition(GameTime gameTime, GraphicsDeviceManager graphics, List<Rectangle> WalkableArea)
        {
            prevPosition = position;

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

            GetMoved(gameTime);
        }

        /*
         * Update the hitbox after a collision.
         */
        public void UpdateHitbox(SkeletonBounds updatedHitbox)
        {
            hitbox = updatedHitbox;
        }

        /*
         * Update the entity position, animation state and skeleton.
         */
        public virtual void UpdateAnimations(GameTime gameTime)
        {
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
         * Set entity position to the centre of the screen +/- any x,y axis adjustment.
         */
        public void SetPosition(float xAdjustment, float yAdjustment)
        {
            position = new Vector2(xAdjustment, yAdjustment);
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

        /*
         * Get the entity position.
         */
        public Vector2 GetPosition()
        {
            return position;
        }

        /*
         * Get the current hitpoints.
         */
        public int GetHitPoints()
        {
            return hitpoints;
        }
    }
}