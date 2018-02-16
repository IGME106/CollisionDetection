using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

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
        //private Rectangle marioPosition;
        private Vector2 marioPosition;
        int numSpritesInSheet;
        int widthOfSingleSprite;
        
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

        // Mario Size
        private int height = 0;
        private int width = 0;

        // Mario Position
        //private int xPosPlayerPlayer = 200;
        //private int yPosPlayerPlayer = 200;
        private float xPosPlayer = 200;
        private float yPosPlayer = 200;

        private MarioState MarioPreviousState = MarioState.Stand;
        private MarioState MarioCurrentState = MarioState.Stand;

        // Fireball texture stuuf
        private Texture2D fireball;
        private Vector2 fireballPosition;

        // Fireball Position
        private float xPosEnemy = 400;
        private float yPosEnemy = 400;

        

        // Animation reqs
        int currentFrame;
        double fps;
        double secondsPerFrame;
        double timeCounter;

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
            widthOfSingleSprite = marioTexture.Width / numSpritesInSheet;

            marioPosition = new  Vector2(xPosPlayer, yPosPlayer);

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

            ProcessInput();
            //UpdateState();

            // Update the animation
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
                                   widthOfSingleSprite * currentFrame,
                                   0,
                                   widthOfSingleSprite,
                                   marioTexture.Height);
            }
            else if (MarioCurrentState == MarioState.FaceRight ||       // Size definition if Mario is facing right
                MarioCurrentState == MarioState.WalkRight)
            {
                currentMario = new Rectangle(
                                   widthOfSingleSprite * currentFrame,
                                   0,
                                   widthOfSingleSprite,
                                   marioTexture.Height);
            }
            else                                                        // If size definition if Mario is not moving
            {
                currentMario = new Rectangle(
                                   0,
                                   0,
                                   widthOfSingleSprite,
                                   marioTexture.Height);
            }
            
            spriteBatch.Draw(                                           // Draw the sprite from the spriteBatch
                marioTexture,
                marioPosition,
                currentMario,
                Color.White,
                0.0f,
                Vector2.Zero,
                1.0f,
                flips,
                0.0f);
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
                xPosPlayer = xPosPlayer - 5;

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
                xPosPlayer = xPosPlayer + 5;

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
                yPosPlayer = yPosPlayer - 5;
            }

            if (keyboardState.IsKeyDown(Keys.S))                    // Move down
            {
                yPosPlayer = yPosPlayer + 5;
            }

            if (keyboardState.IsKeyUp(Keys.W) &&                    // If all keys are up, Mario is standing
                keyboardState.IsKeyUp(Keys.A) &&
                keyboardState.IsKeyUp(Keys.S) &&
                keyboardState.IsKeyUp(Keys.D))
            {
                MarioCurrentState = MarioState.Stand;
            }

            marioPosition = new Vector2(xPosPlayer, yPosPlayer);                // Update characterPosition variable
        }
    }
}
