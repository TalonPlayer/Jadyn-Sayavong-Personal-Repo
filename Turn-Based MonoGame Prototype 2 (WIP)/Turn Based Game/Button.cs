using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;

namespace Turn_Based_Game
{
    internal class Button
    {
        private Texture2D asset;
        private Rectangle position;
        private MouseState previousMouse;

        private Color color;
        private List<Color> colorList;

        private SpriteFont font;
        private string text;
        private Color fontColor;

        private bool disable;

        protected bool click;
        public Button(Texture2D asset, Rectangle position, List<Color> colorList,
            SpriteFont font, string text, Color fontColor) 
        {
            this.asset = asset;
            this.position = position;
            DefaultColor = colorList[0];

            this.colorList = colorList;

            this.font = font;
            this.text = text;
            this.fontColor = fontColor;

            click = false;

            disable = false;
        }

        public Rectangle Position { get { return position; } }

        public Color DefaultColor { get; }
        public bool Click { get { return click; } }
        public string Text { get { return text; } }

        public void Update(GameTime gameTime)
        {
            MouseState mouse = Mouse.GetState();
            if (!disable)
            {
                if (Position.Contains(mouse.Position))
                {
                    color = colorList[1];
                    if (mouse.LeftButton == ButtonState.Pressed)
                    {
                        color = colorList[2];
                        if (previousMouse.LeftButton == ButtonState.Released)
                        {
                            click = true;
                            Debug.WriteLine("Click!");
                        }
                    }
                }
                else
                {
                    color = DefaultColor;
                }
                previousMouse = mouse;
            }
        }

        public void Reset()
        {
            click = false;
            color = DefaultColor;
        }

        public void Selected()
        {
            color = colorList[2];
        }

        public void Disable()
        {
            color = Color.Gray;
            disable = true;
        }

        public void Enable()
        {
            color = DefaultColor;
            disable = false;
        }

        public void Draw(SpriteBatch sb)
        {
            Vector2 textSize = font.MeasureString(text);

            Vector2 location = new Vector2(
                (position.X + position.Width / 2) - textSize.X / 2,
                (position.Y + position.Height / 2) - textSize.Y / 2);

            sb.Draw(asset, position, color);
            sb.DrawString(font, text, 
                location, 
                fontColor);
        }
    }
}
