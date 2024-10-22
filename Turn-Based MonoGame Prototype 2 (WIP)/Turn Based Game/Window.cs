using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;
using System.Security.AccessControl;

namespace Turn_Based_Game
{
    internal class Window
    {
        private Texture2D windowAsset;
        private Rectangle position;
        private Color color;

        private Texture2D buttonAsset;

        private bool active;
        
        private Button open;
        private Button close;
        public Window(Texture2D windowAsset, Rectangle position, Color color,
            Texture2D buttonAsset, List<Color> colorList,
            string buttonText, Rectangle openPos,
            Rectangle closePos, SpriteFont font) 
        { 
            this.windowAsset = windowAsset;
            this.position = position;
            this.color = color;
            this.buttonAsset = buttonAsset;
            active = false;

            open = new Button(buttonAsset, openPos, colorList, font, buttonText, Color.Black);
            close = new Button(buttonAsset, closePos, colorList, font, "X", Color.Black);

        }

        public bool Active { get { return active; } }

        public void Update(GameTime gameTime)
        {
            open.Update(gameTime);

            close.Update(gameTime);

            active = open.Click;

            if (close.Click && active)
            {
                Close();
            }
        }

        public void Close()
        {
            active = false;
            open.Reset();
            close.Reset();
        }

        public void Open(Character selected)
        {
            if (selected.Selected && close.Click)
            {
                selected.Close();
                Close();
            }
            else
            {
                active = selected.Open();
            }
            
        }

        public void Draw(SpriteBatch sb)
        {
            if (active)
            {
                sb.Draw(windowAsset, position, color);
                close.Draw(sb);
            }
            open.Draw(sb);
        }
    }
}
