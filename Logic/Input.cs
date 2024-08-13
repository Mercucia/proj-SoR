﻿using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using SoR.Logic.Entities;

namespace SoR.Logic
{
    internal class Input : Player
    {
        private KeyboardState keyState;
        private KeyboardState lastKeyState;
        protected bool keyPressed;
        private bool enterPressed;
        private bool enterReleased;
        private float speed;
        private int deadZone;
        protected string lastKey;

        public Input(Game1 game) : base(game)
        {
            keyState = Keyboard.GetState();
            enterPressed = keyState.IsKeyDown(Keys.Enter);
            enterReleased = keyState.IsKeyUp(Keys.Enter);
            lastKey = "down";
            keyPressed = false;
            speed = 400f;
            deadZone = 4096;
        }

        public KeyboardState GetKeyState()
        {
            return keyState;
        }

        public void GetUserInput(GameTime gameTime, Player player)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            //Anims: fdown, fdownidle, fside, fsideidle, fup, fupidle, mdown, mdownidle, mside, msideidle, mup, mupidle

            if (keyState.IsKeyDown(Keys.Up))
            {
                keyPressed = true;
                position.Y -= speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                lastKey = "up";
            }

            if (keyState.IsKeyDown(Keys.Down))
            {
                keyPressed = true;
                position.Y += speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                lastKey = "down";
            }

            if (keyState.IsKeyDown(Keys.Left))
            {
                keyPressed = true;
                position.X -= speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                lastKey = "left";
            }

            if (keyState.IsKeyDown(Keys.Right))
            {
                keyPressed = true;
                position.X += speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                lastKey = "right";
            }

            if (Joystick.LastConnectedIndex == 0)
            {
                JoystickState jstate = Joystick.GetState(0);

                float updatedcharSpeed = speed * (float)gameTime.ElapsedGameTime.TotalSeconds;

                if (jstate.Axes[1] < -deadZone)
                {
                    position.Y -= updatedcharSpeed;
                }
                else if (jstate.Axes[1] > deadZone)
                {
                    position.Y += updatedcharSpeed;
                }

                if (jstate.Axes[0] < -deadZone)
                {
                    position.X -= updatedcharSpeed;
                }
                else if (jstate.Axes[0] > deadZone)
                {
                    position.X += updatedcharSpeed;
                }
            }

            lastKeyState = keyState;
        }

        public void UpdateInput()
        {
            keyPressed = lastKeyState.Equals(keyState.IsKeyDown(Keys.Enter)) == enterPressed
                && keyState.Equals(keyState.IsKeyDown(Keys.Enter)) == enterReleased;

            lastKeyState = keyState;
        }
    }
}