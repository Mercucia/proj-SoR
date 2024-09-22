﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using SoR.Logic.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SoR.Logic.Input
{
    /*
     * This class handles player input and animation application.
     */
    public class Movement
    {
        private Dictionary<Keys, InputKeys> inputKeys;
        protected Random random;
        private Vector2 newPosition;
        private Vector2 prevPosition;
        private Vector2 direction;
        private int deadZone;
        private bool idle;
        private string lastPressedKey;
        private int turnAround;
        private string animation;
        private float sinceLastChange;
        private float newDirectionTime;
        private bool traversable;
        public int CountDistance { get; set; }
        public KeyboardState KeyState { get; set; }
        public KeyboardState LastKeyState { get; set; }

        public Movement()
        {
            random = new Random();

            deadZone = 4096; // Set the joystick deadzone
            idle = true; // Player is currently idle
            lastPressedKey = ""; // Get the last key pressed

            traversable = true; // Whether the entity is on walkable terrain

            CountDistance = 0; // Count how far to automatically move the entity
            direction = new Vector2(0, 0); // The direction of movement
            sinceLastChange = 0; // Time since last NPC direction change
            newDirectionTime = (float)random.NextDouble() * 1f + 0.25f; // After 0.25-1 seconds, NPC chooses a new movement direction

            // Dictionary to store the input keys, whether they are currently up or pressed, and which animation to apply
            // TO DO: Simplify to remove duplicated code
            inputKeys = new Dictionary<Keys, InputKeys>()
            {
            { Keys.Up, new InputKeys(KeyState.IsKeyDown(Keys.Up), "runup") },
            { Keys.W, new InputKeys(KeyState.IsKeyDown(Keys.W), "runup") },
            { Keys.Down, new InputKeys(KeyState.IsKeyDown(Keys.Down), "rundown") },
            { Keys.S, new InputKeys(KeyState.IsKeyDown(Keys.S), "rundown") },
            { Keys.Left, new InputKeys(KeyState.IsKeyDown(Keys.Left), "runleft") },
            { Keys.A, new InputKeys(KeyState.IsKeyDown(Keys.A), "runleft") },
            { Keys.Right, new InputKeys(KeyState.IsKeyDown(Keys.Right), "runright") },
            { Keys.D, new InputKeys(KeyState.IsKeyDown(Keys.D), "runright") }
            };
        }

        /* 
         * Move left or right, and adjust animation accordingly.
         * 
         * TO DO?:
         * Adjust to retain current track number for incoming animations.
         * JSON files have exact times for frame starts if hardcoding.
         * AnimationState does return frame start times too, if puzzling out the API.
         * Fix this - possibly switch to idle animation while two opposing direction keys are
         * being held down with no other directional keys, and make player face the direction
         * of travel if 3 buttons held down simultaneously.
         */
        public void CheckMovement(
            GameTime gameTime,
            float speed,
            Vector2 position)
        {
            float newSpeed = speed * (float)gameTime.ElapsedGameTime.TotalSeconds;

            KeyState = Keyboard.GetState(); // Get the current keyboard state

            newPosition = position;

            if (inputKeys.Values.All(inputKeys => !inputKeys.Pressed)) // If no keys are being pressed
            {
                if (!idle) // If idle animation is not currently playing
                {
                    idle = true; // Idle is now playing
                    animation = "idlebattle"; // Set idle animation
                }
            }

            // Set player animation and position according to keyboard input
            foreach (var key in inputKeys.Keys)
            {
                bool pressed = KeyState.IsKeyDown(key);
                bool previouslyPressed = LastKeyState.IsKeyDown(key);
                inputKeys[key].Pressed = pressed;

                if (pressed)
                {
                    if (inputKeys[key].NextAnimation == "runleft")
                    {
                        newPosition.X -= newSpeed;
                    }
                    else if (inputKeys[key].NextAnimation == "runright")
                    {
                        newPosition.X += newSpeed;
                    }

                    if (inputKeys[key].NextAnimation == "runup")
                    {
                        newPosition.Y -= newSpeed;
                    }
                    else if (inputKeys[key].NextAnimation == "rundown")
                    {
                        newPosition.Y += newSpeed;
                    }

                    idle = false; // Idle will no longer be playing

                    lastPressedKey = inputKeys[key].NextAnimation;
                }

                if (pressed & !previouslyPressed)
                {
                    animation = inputKeys[key].NextAnimation;
                }

                if (!pressed & previouslyPressed)
                {
                    animation = lastPressedKey;
                }
            }
            prevPosition = position;

            LastKeyState = KeyState; // Get the previous keyboard state
        }

        /*
         * Check the player is on walkable terrain.
         */
        public void CheckIfTraversable(
            GameTime gameTime,
            Entity entity,
            List<Rectangle> WalkableArea,
            int entityType)
        {
            float newSpeed = (float)(entity.Speed * 1.5) * (float)gameTime.ElapsedGameTime.TotalSeconds;

            foreach (Rectangle area in WalkableArea)
            {
                if (area.Contains(newPosition))
                {
                    traversable = true;
                    break;
                }
                traversable = false;
            }

            if (!traversable)
            {
                direction = Vector2.Zero;
                switch (entityType)
                {
                    case 0: // The player encounters non-traversable terrain
                        Repel(prevPosition, 1, entity);
                        break;
                    case 1: // An NPC encounters non-traversable terrain
                        RedirectNPC(prevPosition, entity);
                        newPosition += direction * newSpeed;
                        break;
                    case 2:
                        RedirectPlayer(); // The player is pushed into non-traversable terrain
                        break;
                }
            }
        }

        /*
         * Decide which direction the player is pushed in depending on local environment.
         */
        public void RedirectPlayer()
        {
            /*
             * Shift the player's movement laterally if they hit a wall or other
             * non-traversable object.
             */


        }

        /*
         * Change direction to move away from something.
         */
        public void RedirectNPC(Vector2 location, Entity entity)
        {
            if (newPosition.X > location.X) // Moving right
            {
                direction = new Vector2(-1, 0); // Redirect left
                RedirectAnimation(1, entity);
            }
            if (newPosition.X < location.X) // Moving left
            {
                direction = new Vector2(1, 0); // Redirect right
                RedirectAnimation(2, entity);
            }
            if (newPosition.Y > location.Y) // Moving down
            {
                direction = new Vector2(0, -1); // Redirect up
            }
            if (newPosition.Y < location.Y) // Moving up
            {
                direction = new Vector2(0, 1); // Redirect down
            }
        }

        /*
         * Animate NPC redirection.
         */
        public void RedirectAnimation(int newDirection, Entity entity)
        {
            switch (newDirection)
            {
                case 1:
                    entity.ChangeAnimation("turnleft");
                    entity.GetSkeleton().ScaleX = 1;
                    break;
                case 2:
                    entity.ChangeAnimation("turnright");
                    entity.GetSkeleton().ScaleX = -1;
                    break;
            }
        }

        /*
         * Choose a new direction to face.
         */
        public void NewDirection(Entity entity, int newDirection)
        {
            switch (newDirection)
            {
                case 1:
                    direction = new Vector2(-1, 0); // Left
                    RedirectAnimation(1, entity);
                    break;
                case 2:
                    direction = new Vector2(1, 0); // Right
                    RedirectAnimation(2, entity);
                    break;
                case 3:
                    direction = new Vector2(0, -1); // Up
                    break;
                case 4:
                    direction = new Vector2(0, 1); // Down
                    break;
            }
        }

        /*
         * Move the NPC in the direction they're facing, and periodically pick a random new direction.
         */
        public void NonPlayerMovement(GameTime gameTime, Entity entity)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            float newSpeed = entity.Speed * deltaTime;
            int newDirection = 0;
            sinceLastChange += deltaTime;
            newPosition = entity.GetPosition();

            if (entity.IsMoving())
            {
                if (sinceLastChange >= newDirectionTime)
                {
                    newDirection = random.Next(4);
                    NewDirection(entity, newDirection);
                    newDirectionTime = (float)random.NextDouble() * 3f + 0.33f;
                    sinceLastChange = 0;
                }

                newPosition += direction * newSpeed;

                if (!traversable)
                {

                }
            }
            prevPosition = entity.GetPosition();
        }

        /*
         * Repel an entity away.
         */
        public void Repel(Vector2 location, int distance, Entity entity)
        {
            CountDistance = distance;
            while (CountDistance < distance)
            {
                CountDistance++;
            }

            direction = Vector2.Zero;

            if (entity.GetPosition().X > location.X) // Push right
            {
                direction.X += 1;
            }
            else if (entity.GetPosition().X < location.X) // Push left
            {
                direction.X -= 1;
            }
            if (entity.GetPosition().Y > location.Y) // Push down
            {
                direction.Y += 1;
            }
            else if (entity.GetPosition().Y < location.Y) // Push up
            {
                direction.Y -= 1;
            }

            entity.UpdateDirection(direction);
        }

        /*
         * Return the animation to play according to user input.
         */
        public string AnimateMovement()
        {
            return animation;
        }

        /*
         * Return the current direction to face. 1 = left, 2 = right, 3 = up, 4 = down.
         */
        public int TurnAround()
        {
            return turnAround;
        }

        /*
         * Change the player's position on the screen according to joypad inputs
         */
        public void ProcessJoypadInputs(GameTime gameTime, float speed)
        {
            if (Joystick.LastConnectedIndex == 0)
            {
                JoystickState jstate = Joystick.GetState(0);

                float newPlayerSpeed = speed * (float)gameTime.ElapsedGameTime.TotalSeconds;

                if (jstate.Axes[1] < -deadZone)
                {
                    newPosition.Y -= newPlayerSpeed;
                }
                else if (jstate.Axes[1] > deadZone)
                {
                    newPosition.Y += newPlayerSpeed;
                }

                if (jstate.Axes[0] < -deadZone)
                {
                    newPosition.X -= newPlayerSpeed;
                }
                else if (jstate.Axes[0] > deadZone)
                {
                    newPosition.X += newPlayerSpeed;
                }
            }
        }

        /*
         * Set the direction to be moved in.
         */
        public void SetDirection(float x, float y)
        {
            direction.X = x;
            direction.Y = y;
        }

        /*
         * Get the current movement direction.
         */
        public Vector2 GetDirection()
        {
            return direction;
        }

        /*
         * Get the new x-axis position.
         */
        public Vector2 UpdatePosition()
        {
            return newPosition;
        }
    }
}