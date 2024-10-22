using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Threading;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;

namespace Turn_Based_Game
{
    
    abstract class Character : Entity
    {
        private Texture2D buttonAsset;
        private SpriteFont font;
        private Texture2D attackButtons;
        private List<Color> buttonColors;

        protected Enemy enemy;

        private bool selected;

        protected double timer;

        private Rectangle defaultPos;

        private Button open;

        protected bool moving;

        protected Dictionary<int, bool> selectedAction;

        protected int attacks;

        protected int speed;

        protected List<string> abilityNames;

        protected bool openWindow;

        protected Button stats;

        public Character(Texture2D asset, Rectangle position, string name, int level) :
            base(asset, position, name, level)
        {
            selected = false;
            defaultPos = position;
            timer = 0;
            speed = 0;
            openWindow = false;
        }

        public bool OpenWindow { get { return openWindow; } }
        public bool Selected {  get { return selected; } }

        public Button Stats { get { return stats; } }

        public Rectangle DefaultPos { get { return defaultPos; } }

        public override void Update(GameTime gameTime)
        {

            open.Update(gameTime);

            selected = open.Click;

        }


        public void Close()
        {
            selected = false;

            open.Reset();

        }

        public bool Open()
        {
            return open.Click;
        }

        public void Return()
        {
            position.X -= speed;

            if (position.X < defaultPos.X)
            {
                speed = 0;
                position.X = defaultPos.X;
            }

            position.Y = defaultPos.Y;
        }

        public virtual void SelectMember()
        {
        
        }

        public void LoadAssets(Texture2D b, SpriteFont f, Texture2D a, 
            List<Color> colorList , List<Color> buttonColors, Rectangle s)
        {
            buttonAsset = b;
            font = f;
            attackButtons = a;

            stats = new Button(a, s, colorList, f, "", Color.Black);

            this.buttonColors = buttonColors;

            abilitiesList = FillButtons(abilityNames);

            open = new Button(buttonAsset, Position, colorList, font, " ", Color.Black);

        }

        public Dictionary<Button, Rectangle> FillButtons(List<string> name)
        {
            Dictionary<Button, Rectangle> dict = new Dictionary<Button, Rectangle>();
            for (int i = 1; i <= name.Count; i++)
            {
                Button button = new Button(
                    attackButtons,
                    new Rectangle(
                        150 + (275 * ((i - 1) % 2)),
                        200 + (100 * ((i - 1) / 2)),
                150, 75),
                    buttonColors, font, name[i - 1], Color.Black);

                Rectangle picPosition =
                    new Rectangle(
                        button.Position.X - 100, button.Position.Y,
                        75, 75);

                dict.Add(button, picPosition);
            }

            return dict;
        }

        public override void Draw(SpriteBatch sb, Color tint)
        {
            if (Alive)
            {
                base.Draw(sb, tint);
                if (timer < 0)
                {
                    open.Draw(sb);
                }
            }
        }
    }
}
