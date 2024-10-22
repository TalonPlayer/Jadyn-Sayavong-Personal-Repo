using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Ball_Game_Redo
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private Texture2D ball;
        private Texture2D brick;

        private SpriteFont font;

        private Vector2 velocity1;
        private Vector2 velocity2;

        private Rectangle ballPos;
        private Rectangle brickPos;

        private Rectangle brickPos2;

        private List<Rectangle> bricks;

        private double timer;

        private KeyboardState currentKey;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            font = Content.Load<SpriteFont>("Arial26");

            ball = Content.Load<Texture2D>("ball");

            brick = Content.Load<Texture2D>("brick");

            ballPos = new Rectangle(375, 400, 20,20);

            brickPos = new Rectangle(300, 300, 50, 25);

            brickPos2 = new Rectangle(350, 100, 50, 50);

            bricks = new List<Rectangle>();

            velocity1 = new Vector2(-1, -1);

            bricks.Add(brickPos);
            bricks.Add(brickPos2);

            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here

            currentKey = Keyboard.GetState();

            if (ballPos.Bottom >= brickPos.Top && ballPos.Bottom < brickPos.Bottom && InXRegion(brickPos, ballPos))
            {
                velocity1.Y = -1;
                ballPos.Y = brickPos.Top;
            }

            if (ballPos.Right >= brickPos.Left && ballPos.Right < brickPos.Right && InYRegion(brickPos, ballPos))
            {
                velocity1.X = -1;
                ballPos.X = brickPos.Left;
            }

            if (ballPos.Top <= brickPos.Bottom && ballPos.Top > brickPos.Top && InXRegion(brickPos, ballPos))
            {
                velocity1.Y = 1;
                ballPos.Y = brickPos.Bottom;
            }

            if (ballPos.Left <= brickPos.Right && ballPos.Left > brickPos.Left && InYRegion(brickPos, ballPos))
            {
                velocity1.X = 1;
                ballPos.X = brickPos.Right;
            }

            if (currentKey.IsKeyDown(Keys.Space))
            {
                ballPos = Move(ballPos, velocity1);
            }
            base.Update(gameTime);
        }

        public bool InXRegion(Rectangle brickPos, Rectangle ballPos)
        {
            // The left of the ball is more than the position of the brick's left, and
            // The right of the ball is less than the position of the brick's right.

            // return true if the ball's X values are within the region of the brick's X values
            // false if otherwise
            return ballPos.Left >= brickPos.Left && ballPos.Right <= brickPos.Right; ;
        }

        public bool InYRegion(Rectangle brickPos, Rectangle ballPos)
        {
            
            return ballPos.Top >= brickPos.Top && ballPos.Bottom <= brickPos.Bottom;
        }

        public Rectangle Move(Rectangle ballPos, Vector2 velocity)
        {
            return new Rectangle(
                ballPos.X += 3 * (int) velocity.X,
                ballPos.Y += 8 * (int) velocity.Y,
                ballPos.Width, ballPos.Height
                );
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            _spriteBatch.Begin();

            _spriteBatch.Draw(ball, ballPos, Color.White);

            _spriteBatch.Draw(brick, brickPos, Color.White);

            _spriteBatch.Draw(brick, brickPos2, Color.White);

            _spriteBatch.End();

            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }
    }
}