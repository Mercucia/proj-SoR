﻿using Spine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;

namespace SoR.Logic.Input
{
    /*
     * This class handles player input and animation application.
     */
    public class PlayerInput
    {
        private KeyboardState keyState;
        private KeyboardState lastKeyState;
        private int deadZone;
        private float speed;
        private float newPositionX;
        private float newPositionY;
        private bool switchSkin;

        public PlayerInput()
        {
            // Set the joystick deadzone
            deadZone = 4096;
        }

        /*
         * Set the current running animation and move the player character across the screen according
         * to keyboard inputs. Set back to the idle animation if there are no current valid movement
         * inputs.
         */
        public void ProcessKeyboardInputs(
            GameTime gameTime,
            AnimationState animState,
            float speed,
            float positionX,
            float positionY)
        {
            //Anims: fdown, fdownidle, fside, fsideidle, fup, fupidle, mdown, mdownidle, mside, msideidle, mup, mupidle

            keyState = Keyboard.GetState(); // Get the current keyboard state

            // Dictionary to store the input keys and whether they are currently up or pressed.
            Dictionary<Keys, bool> keyIsUp =
                new Dictionary<Keys, bool>()
                {
                    { Keys.Up, keyState.IsKeyUp(Keys.Up) },
                    { Keys.Down, keyState.IsKeyUp(Keys.Down) },
                    { Keys.Left, keyState.IsKeyUp(Keys.Left) },
                    { Keys.Right, keyState.IsKeyUp(Keys.Right) }
                };

            float newPlayerSpeed = speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            bool travelUp = false;
            bool travelDown = false;
            bool travelLeft = false;
            bool travelRight = false;

            this.speed = speed;
            newPositionX = positionX;
            newPositionY = positionY;

            switchSkin = false; // Space has not been pressed yet, the skin will not be switched

            /* Set player animation and position according to keyboard input.
             * 
             * TO DO?:
             * Adjust to retain current track number for incoming animations.
             * JSON files have exact times for frame starts if hardcoding.
             * AnimationState does return frame start times too, if puzzling out the API.
             */
            if (keyState.IsKeyDown(Keys.Up))
            {
                newPositionY -= newPlayerSpeed;
                if (!lastKeyState.IsKeyDown(Keys.Up))
                {
                    animState.SetAnimation(0, "runup", true);
                    travelUp = true;
                }
            }
            if (keyState.IsKeyDown(Keys.Down))
            {
                newPositionY += newPlayerSpeed;
                if (!lastKeyState.IsKeyDown(Keys.Down))
                {
                    animState.SetAnimation(0, "rundown", true);
                    travelDown = true;
                }
            }
            if (keyState.IsKeyDown(Keys.Left))
            {
                newPositionX -= newPlayerSpeed;
                if (!lastKeyState.IsKeyDown(Keys.Left))
                {
                    animState.SetAnimation(0, "runleft", true);
                    travelLeft = true;
                }
            }
            if (keyState.IsKeyDown(Keys.Right))
            {
                newPositionX += newPlayerSpeed;
                if (!lastKeyState.IsKeyDown(Keys.Right))
                {
                    animState.SetAnimation(0, "runright", true);
                    travelRight = true;
                }
            }

            /*
             * If a key has just been released, set the running animation back to the direction the character
             * is currently moving in. If two keys are being pressed simultaneously, set it to the direction
             * of the most recently pressed key.
             * 
             * TO DO: Fix this - doesn't quite work as intended and needs simplifying. Also need to switch to
             * idle animation while two opposing direction keys are being held down with no other directional
             * keys.
             */
            foreach (var key in keyIsUp)
            {
                if (key.Value & lastKeyState.IsKeyDown(key.Key))
                {
                    if (keyState.IsKeyDown(Keys.Right) &
                        !keyState.IsKeyDown(Keys.Left))
                    {
                        animState.SetAnimation(0, "runright", true);
                    }
                    if (keyState.IsKeyDown(Keys.Left) &
                        !keyState.IsKeyDown(Keys.Right))
                    {
                        animState.SetAnimation(0, "runleft", true);
                    }
                    if (keyState.IsKeyDown(Keys.Down) &
                        !keyState.IsKeyDown(Keys.Up))
                    {
                        animState.SetAnimation(0, "rundown", true);
                    }
                    if (keyState.IsKeyDown(Keys.Up) &
                        !keyState.IsKeyDown(Keys.Down))
                    {
                        animState.SetAnimation(0, "runup", true);
                    }
                    else if (!keyIsUp.ContainsValue(false))
                    {
                        animState.SetAnimation(0, "idlebattle", true);
                    }
                }
            }

            if (keyState.IsKeyDown(Keys.Space) & !lastKeyState.IsKeyDown(Keys.Space))
            {
                switchSkin = true; // Space was pressed, so switch skins
            }

            lastKeyState = keyState; // Get the previous keyboard state
        }

        /*
         * Handle environmental collision. Currently just the edge of the game window.
         */
        public void EnvironCollision(
            GraphicsDeviceManager graphics,
            GraphicsDevice GraphicsDevice,
            float positionX,
            float positionY)
        {
            newPositionX = positionX;
            newPositionY = positionY;

            if (newPositionX > graphics.PreferredBackBufferWidth - 5)
            {
                newPositionX = graphics.PreferredBackBufferWidth - 5;
            }
            else if (newPositionX < 5)
            {
                newPositionX = 5;
            }

            if (newPositionY > graphics.PreferredBackBufferHeight - 8)
            {
                newPositionY = graphics.PreferredBackBufferHeight - 8;
            }
            else if (newPositionY < 8)
            {
                newPositionY = 8;
            }
        }

        /*
         * Change the player's position on the screen according to joypad inputs
         */
        public void ProcessJoypadInputs(GameTime gameTime, float speed)
        {
            this.speed = speed;

            if (Joystick.LastConnectedIndex == 0)
            {
                JoystickState jstate = Joystick.GetState(0);

                float updatedcharSpeed = speed * (float)gameTime.ElapsedGameTime.TotalSeconds;

                if (jstate.Axes[1] < -deadZone)
                {
                    newPositionY -= updatedcharSpeed;
                }
                else if (jstate.Axes[1] > deadZone)
                {
                    newPositionY += updatedcharSpeed;
                }

                if (jstate.Axes[0] < -deadZone)
                {
                    newPositionX -= updatedcharSpeed;
                }
                else if (jstate.Axes[0] > deadZone)
                {
                    newPositionX += updatedcharSpeed;
                }
            }
        }

        /*
         * Get the new x-axis position.
         */
        public float UpdatePositionX()
        {
            return newPositionX;
        }

        /*
         * Get the new y-axis position.
         */
        public float UpdatePositionY()
        {
            return newPositionY;
        }

        /*
         * Check whether space was pressed and the skin should change.
         */
        public bool SkinHasChanged()
        {
            return switchSkin;
        }
    }
}