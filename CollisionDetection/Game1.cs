using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace CollisionDetection
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        
        // Mario texture stuff
        private Texture2D marioTexture;
        private Rectangle marioSprite;
        int numSpritesInSheet;
        int marioWidth = 44;
        int marioHeight = 72;
        
        // Mario Statemachine
        private enum MarioState
        {
            WalkLeft,
            FaceLeft,
            Stand,
            FaceRight,
            WalkRight,
            Jump
        }

        // Mario Position
        private int xPosPlayer = 0;
        private int yPosPlayer = 0;

        private MarioState MarioPreviousState = MarioState.Stand;
        private MarioState MarioCurrentState = MarioState.Stand;

        // Fireball texture stuff
        private Texture2D fireballTexture;
        private List<Rectangle> rectanglesSprite = new List<Rectangle>();
        private int nrOfFireballs = 5;
        private int drawInterval = 0;
        private int previousChangeInX = 5;
        private int previousChangeInY = 5;

        private int fireballHeight = 50;
        private int fireballWidth = 50;
        
        // Animation reqs
        int currentFrame;
        double fps;
        double secondsPerFrame;
        double timeCounter;

        // Random location generator
        public static Random random = new Random();
        private static readonly RNGCryptoServiceProvider generator = new RNGCryptoServiceProvider();

        /// <summary>
        /// Main program for game
        /// </summary>
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            Window.Position = new Point(                    // Center the game view on the screen
                (GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width / 2) -
                    (graphics.PreferredBackBufferWidth / 2),
                (GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height / 2) -
                    (graphics.PreferredBackBufferHeight / 2)
            );
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            marioSprite = new Rectangle(xPosPlayer, yPosPlayer, marioWidth, marioHeight);
            
            for (int i = 0; i < nrOfFireballs; i++)
            {
                int randomX = (int)random.Next(GraphicsDevice.Viewport.Width);
                int randomY = (int)random.Next(GraphicsDevice.Viewport.Height);

                rectanglesSprite.Add(new Rectangle(randomX, randomY, fireballWidth, fireballHeight));
            }

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            marioTexture = Content.Load<Texture2D>("MarioSpriteSheet");
            numSpritesInSheet = 4;
            marioWidth = marioTexture.Width / numSpritesInSheet;

            marioSprite = new Rectangle(xPosPlayer, yPosPlayer, marioWidth, marioHeight);

            fireballTexture = Content.Load<Texture2D>("FireballSprite");

            // Set up animation stuff
            currentFrame = 1;
            fps = 10.0;
            secondsPerFrame = 1.0f / fps;
            timeCounter = 0;
        }


        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // *** Put code to check and update FINITE STATE MACHINE here

            UpdateFireballs();

            ProcessInput();
            
            UpdateAnimation(gameTime);
            base.Update(gameTime);
        }

        /// <summary>
        /// Updates the animation time
        /// </summary>
        /// <param name="gameTime">Game time information</param>
        private void UpdateAnimation(GameTime gameTime)
        {
            // Add to the time counter (need TOTALSECONDS here)
            timeCounter += gameTime.ElapsedGameTime.TotalSeconds;

            // Has enough time gone by to actually flip frames?
            if (timeCounter >= secondsPerFrame)
            {
                // Update the frame and wrap
                currentFrame++;
                if (currentFrame >= 4)
                    currentFrame = 1;

                // Remove one "frame" worth of time
                timeCounter -= secondsPerFrame;
            }
        }

        /// <summary>
        /// Generates random direction for fireballs to move into
        /// </summary>
        protected void UpdateFireballs()
        {
            int randomDirection = 0;
            int randomX = 0;
            int randomY = 0;

            if (drawInterval == 200)
            {
                for (int i = 0; i < nrOfFireballs; i++)
                {

                    randomX = rectanglesSprite[i].X;
                    randomY = rectanglesSprite[i].Y;

                    randomDirection = NumberBetween(0, 800);                    // Decide on direction depending on random number received
                    Console.WriteLine(randomDirection);

                    if ((randomDirection > 0) && (randomDirection <= 100))
                    {
                        Console.WriteLine("1");
                        randomX += 5;

                        previousChangeInX = 5;
                        previousChangeInY = 0;
                    }
                    else if ((randomDirection > 101) && (randomDirection <= 200))
                    {
                        Console.WriteLine("2");
                        randomY += 5;

                        previousChangeInX = 0;
                        previousChangeInY = 5;
                    }
                    else if ((randomDirection > 201) && (randomDirection <= 300))
                    {
                        Console.WriteLine("3");
                        randomX -= 5;

                        previousChangeInX = -5;
                        previousChangeInY = 0;
                    }
                    else if ((randomDirection > 301) && (randomDirection <= 400))
                    {
                        Console.WriteLine("4");
                        randomY -= 5;

                        previousChangeInX = 0;
                        previousChangeInY = -5;
                    }
                    else if ((randomDirection > 401) && (randomDirection <= 500))
                    {
                        Console.WriteLine("5");
                        randomX += 5;
                        randomY += 5;

                        previousChangeInX = 5;
                        previousChangeInY = 5;
                    }
                    else if ((randomDirection > 501) && (randomDirection <= 600))
                    {
                        Console.WriteLine("6");
                        randomX -= 5;
                        randomY -= 5;

                        previousChangeInX = -5;
                        previousChangeInY = -5;
                    }
                    else if ((randomDirection > 601) && (randomDirection <= 700))
                    {
                        Console.WriteLine("7");
                        randomX += 5;
                        randomY -= 5;

                        previousChangeInX = 5;
                        previousChangeInY = -5;
                    }
                    else if ((randomDirection > 701) && (randomDirection <= 800))
                    {
                        Console.WriteLine("8");
                        randomX -= 5;
                        randomY += 5;

                        previousChangeInX = -5;
                        previousChangeInY = 5;
                    }

                    rectanglesSprite[i] = new Rectangle(                        // Draw the fireball with the new properties
                        CheckX(randomX, fireballWidth),
                        CheckY(randomY, fireballHeight),
                        fireballWidth,
                        fireballHeight);

                    drawInterval = 0;
                }
            }
            else
            {   
                for (int i = 0; i < nrOfFireballs; i++)                         // This loop allows the fireballs to move in a 
                {                                                                   // straight line for a while before changing
                    randomX = rectanglesSprite[i].X;                                // direction.
                    randomY = rectanglesSprite[i].Y;

                    rectanglesSprite[i] = new Rectangle(
                    CheckX(randomX + previousChangeInX, fireballWidth),
                    CheckY(randomY + previousChangeInY, fireballHeight),
                    fireballWidth,
                    fireballHeight);

                    drawInterval++;
                }
            }       
        }

        /// <summary>
        /// Checks if the X coordinate is leaving the screen
        /// </summary>
        /// <param name="xValue">X coordinate</param>
        /// <param name="objectWidth">Width of the object</param>
        /// <returns></returns>
        private int CheckX(int xValue, int objectWidth)
        {
            int returnValue = xValue;

            if (xValue > GraphicsDevice.Viewport.Width)
            {
                returnValue = 0;
            }
            else if (xValue < 0)
            {
                returnValue = GraphicsDevice.Viewport.Width;
            }

            return returnValue;
        }

        /// <summary>
        /// Checks if the Y coordinate is leaving the screen
        /// </summary>
        /// <param name="yValue">Y coordinate</param>
        /// <param name="objectHeight">Height of the object</param>
        /// <returns></returns>
        private int CheckY(int yValue, int objectHeight)
        {
            int returnValue = yValue;

            if (yValue > GraphicsDevice.Viewport.Height)
            {
                returnValue = 0;
            }
            else if (yValue < 0)
            {
                returnValue = GraphicsDevice.Viewport.Height;
            }

            return returnValue;
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin();

            // *** Put code to check FINITE STATE MACHINE
            // *** and properly draw mario here

            // Example call to draw mario walking (replace or adjust this line!)
            DrawMario();
            DrawFireballs();

            spriteBatch.End();

            base.Draw(gameTime);
        }
        
        /// <summary>
        /// Determines which way Mario is facing and applies the appropriate
        ///   SpriteEffects.  Also defines the section from the SpriteSheet
        ///   that is to be drawn.
        /// </summary>
        private void DrawMario()
        {
            Rectangle currentMario = new Rectangle();
            SpriteEffects flips = SpriteEffects.None;

            if (MarioCurrentState == MarioState.FaceLeft ||             // Size definition if Mario is facing left
                MarioCurrentState == MarioState.WalkLeft)                
            {
                flips = SpriteEffects.FlipHorizontally;                 // If mario is facing other way, flip sprite

                currentMario = new Rectangle(
                                   marioWidth * currentFrame,
                                   0,
                                   marioWidth,
                                   marioHeight);
            }
            else if (MarioCurrentState == MarioState.FaceRight ||       // Size definition if Mario is facing right
                MarioCurrentState == MarioState.WalkRight)
            {
                currentMario = new Rectangle(
                                   marioWidth * currentFrame,
                                   0,
                                   marioWidth,
                                   marioHeight);
            }
            else                                                        // If size definition if Mario is not moving
            {
                currentMario = new Rectangle(
                                   0,
                                   0,
                                   marioWidth,
                                   marioHeight);
            }

            spriteBatch.Draw(                                           // Draw the sprite from the spriteBatch
                marioTexture,
                marioSprite,
                currentMario,
                Color.White,
                0.0f,
                Vector2.Zero,
                flips,
                0.0f);
        }


        /// <summary>
        /// Draws the fireballs to screen
        /// </summary>
        private void DrawFireballs()
        {
            Color drawColor = new Color();

            for (int i = 0; i < 5; i++)
            {
                if (rectanglesSprite[i].Intersects(marioSprite))
                {
                    drawColor = Color.Red;                              // Display as red when contact with player
                }
                else
                {
                    drawColor = Color.White;                            // Display normal if there is no contact
                }

                spriteBatch.Draw(
                fireballTexture,
                rectanglesSprite[i],
                drawColor);
            }
        }

        /// <summary>
        /// Takes user input and analysis state to define next state.
        /// </summary>
        private void ProcessInput()
        {
            KeyboardState keyboardState = Keyboard.GetState();

            MarioPreviousState = MarioCurrentState;

            if (keyboardState.IsKeyDown(Keys.A))                    // Move left
            {
                xPosPlayer = CheckX(xPosPlayer - 5, marioWidth);

                if (MarioPreviousState == MarioState.WalkRight)
                {
                    MarioCurrentState = MarioState.FaceRight;
                }
                else if (MarioPreviousState == MarioState.FaceRight)
                {
                    MarioCurrentState = MarioState.Stand;
                }
                else if (MarioPreviousState == MarioState.Stand)
                {                    
                    MarioCurrentState = MarioState.FaceLeft;
                }
                else if (MarioPreviousState == MarioState.FaceLeft)
                {
                    MarioCurrentState = MarioState.WalkLeft;
                }
            }

            if (keyboardState.IsKeyDown(Keys.D))                    // Move right
            {
                xPosPlayer = CheckX(xPosPlayer + 5, marioWidth);

                if (MarioPreviousState == MarioState.WalkLeft)
                {
                    MarioCurrentState = MarioState.FaceLeft;
                }
                else if (MarioPreviousState == MarioState.FaceLeft)
                {
                    MarioCurrentState = MarioState.Stand;
                }
                else if (MarioPreviousState == MarioState.Stand)
                {
                    MarioCurrentState = MarioState.FaceRight;
                }
                else if (MarioPreviousState == MarioState.FaceRight)
                {
                    MarioCurrentState = MarioState.WalkRight;
                }
            }

            if (keyboardState.IsKeyDown(Keys.W))                    // Move up
            {
                yPosPlayer = CheckY(yPosPlayer - 5, marioHeight);
            }

            if (keyboardState.IsKeyDown(Keys.S))                    // Move down
            {
                yPosPlayer = CheckY(yPosPlayer + 5, marioHeight);
            }

            if (keyboardState.IsKeyUp(Keys.W) &&                    // If all keys are up, Mario is standing
                keyboardState.IsKeyUp(Keys.A) &&
                keyboardState.IsKeyUp(Keys.S) &&
                keyboardState.IsKeyUp(Keys.D))
            {
                MarioCurrentState = MarioState.Stand;
            }

            marioSprite = new Rectangle(xPosPlayer, yPosPlayer, marioWidth, marioHeight);                // Update characterPosition variable
        }

        /// <summary>
        /// Improved random number generator.  Built in generator doesn' work that well.
        /// produces a number between min and max value including those values
        /// </summary>
        /// <param name="minimumValue">Lowest value to include in generation</param>
        /// <param name="maximumValue">Highest value to include in generation</param>
        /// <returns></returns>
        private static int NumberBetween(int minimumValue, int maximumValue)
        {
            byte[] randomNumber = new byte[1];

            generator.GetBytes(randomNumber);

            double asciiValueOfRandomCharacter = Convert.ToDouble(randomNumber[0]);

                                                            // We are using Math.Max, and substracting 0.00000000001, 
                                                            // to ensure "multiplier" will always be between 0.0 and .99999999999
                                                            // Otherwise, it's possible for it to be "1", which causes problems in our rounding.
            double multiplier = Math.Max(0, (asciiValueOfRandomCharacter / 255d) - 0.00000000001d);

                                                            // We need to add one to the range, to allow for the rounding done with Math.Floor
            int range = maximumValue - minimumValue + 1;

            double randomValueInRange = Math.Floor(multiplier * range);

            return (int)(minimumValue + randomValueInRange);
        }
    }
}
