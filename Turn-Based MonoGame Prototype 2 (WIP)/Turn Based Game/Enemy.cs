using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Diagnostics;
using static System.Net.Mime.MediaTypeNames;

namespace Turn_Based_Game
{
    internal class Enemy : Entity
    {
        private SpriteFont font;
        private Texture2D asset;

        private Random rng = new Random();

        private string name;
        private bool alive;

        private bool message;
        private double timer;

        private List<string> damageTaken = new List<string>();
        private List<Vector2> damagePos = new List<Vector2>();
        public Enemy(Texture2D asset, Rectangle position, string name, int level, int health)
            : base(asset, position, name, level)
        {
            this.asset = asset;
            this.position = position;
            this.health = 1000 * level;

            alive = true;
        }

        public override void Update(GameTime gameTime)
        {
            if (message)
            {
                timer -= gameTime.ElapsedGameTime.TotalSeconds;
                if (timer < 0)
                {
                    message = false;
                    damageTaken.Clear();
                    damagePos.Clear();
                }
            }
        }

        public void LoadAssets(SpriteFont f, Random r)
        {
            font = f;
            rng = r;
        }
        public override void TakeDamage(int damage)
        {
            DrawDamage(damage);
            health -= damage;

            alive = health > 0;
        }

        public void DrawDamage(int damage)
        {
            message = true;
            timer = 2;
            damageTaken.Add("" + damage);
            damagePos.Add(new Vector2(
                rng.Next(position.X, position.X + position.Width), 
                rng.Next(position.Y, position.Y + position.Height)));
        }
        public override void Draw(SpriteBatch sb, Color tint)
        {
            Vector2 textSize = font.MeasureString("" + health);

            Vector2 location = new Vector2(
                (position.X + position.Width / 2) - textSize.X / 2,
                (position.Y + position.Height / 2) - textSize.Y / 2);


            sb.Draw(asset, position, tint);
            sb.DrawString(font, "" + health, location, Color.White);

            if (message)
            {
                for (int i = 0; i < damageTaken.Count; i++)
                {
                    sb.DrawString(font, "" + damageTaken[i], damagePos[i], Color.Black); ;

                }
            }

        }
        public override void Action(Button action, Character character, Entity enemy, int index)
        {
            //
        }

        public override void BasicAttack(int attacks, Enemy enemy)
        {

        }

        public override void Move()
        {

        }

        public override void Abilities(int index)
        {

        }


    }
}
