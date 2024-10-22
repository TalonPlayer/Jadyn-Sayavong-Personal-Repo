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


namespace Turn_Based_Game
{
    internal class Slapstick : Soldier
    {
        /* Slapstick specializes in dealing and boosting imaginary damage
         * 
         * Passive: Let's Paint the World! 
         * Slapstick's basic attack apply a random color to the enemy he hits.
         * If Slapstick crits with an attack, 2 unique paint colors are applied
         * instead
         * 
         * 
         * Paint on an enemy on lasts for 2 turns
         * Applying Paint refreshes this so that every paint will last the same
         * 
         * If slapstick manages to get every unique color on a single enemy,
         * that enemy takes double damage from Slapstick only
         * 
         * Passive: Black Fear Paint
         * Slapstick has an increased critical chance on an enemy with black paint
         *
         * Abilities:
         * (20 S) Color Rush - Slapstick medium attacks an enemy 3 times, applying 3 different random colors
         * 
         * (15 E) Don't Stop - For the next 3 turns, Slapstick takes reduced damage
         * 
         * (35 S) Stop Sign Swirl - Slapstick light attacks an enemy 9 times, and then heavy attacks once. 
         * Applies only 2 colors
         * 
         * (20 E) Colorful High Five - Slapstick's paint passive will be given to a chosen ally for 3 turns
         * 
         * (25 E) Overcome Fears - Slapstick increases regeneration in health, stamina, and health for 3 turns 
         * 
         * (40 E and 30 S) Paint Assault - Slapstick will unleash 2 ranged projectiles after every action for 3 turns
         * Ultimate: Ultimate Color Blast
         * Slapstick blasts all enemies applying every color to every enemy hit
         * Paint now lasts for 5 turns
        */

        private Dictionary<string, bool> paint;

        private double attackPause;

        private int attackIndex;

        private int paintApplied;

        private bool rangePaint;

        private int destination;

        private int multiplier;

        private int addAtt;
        
        public Slapstick(Texture2D asset, Rectangle position, string name, int level) :
            base(asset, position, name, level)
        {
            // Default attack is 1
            maxHealth = health;

            phys = 25;
            magic = 10;

            armor = 5;
            shield = 5;

            paint = new Dictionary<string, bool>
            {
                { "Red", false }, 
                { "Orange", false },
                { "Yellow", false },
                { "Green", false },
                { "Light Blue", false},
                { "Blue", false },
                { "Purple", false },
                { "Magenta", false },
                { "Black", false }
            };

            abilityNames = new List<string> 
            { 
                "Ultimate Color Blast", 
                "Color Rush", 
                "Don't Stop", 
                "Stop Sign Swirl", 
                "Colorful High Five",
                "Overcome Fears",
                "Paint Assault"
            };

            attackPause = 0;
            attackIndex = 0;
            rangePaint = false;
            paintApplied = 0;
            destination = 1000;
            multiplier = 1;
            addAtt = 0;
        }

        public void ApplyColor(int paintApplied)
        {
            int rngIndex = 0;
            for (int i = 0; i < paintApplied; i++)
            {
                rngIndex = rng.Next(0, paint.Count);
                
                if (!CheckColorCount())
                {
                    if (!paint.ElementAt(rngIndex).Value)
                    {
                        paint[paint.ElementAt(rngIndex).Key] = true;
                        Debug.WriteLine(paint.ElementAt(rngIndex).Key);
                        multiplier = 1;
                    }
                    else
                    {
                        i--;
                    }
                }
            }
        }

        public bool CheckColorCount()
        {
            bool all = true;
            foreach (var color in paint)
            {
                if (!color.Value)
                {
                    all = false;
                }
            }

            if (all)
            {
                multiplier = 2;
            }

            if (paint["Black"])
            {
                Debug.WriteLine("Black Paint");
                crit = 30;
            }
            else
            {
                crit = 15;
            }

            return all;
        }

        public override void BasicAttack(int attacks, Enemy enemy)
        {
            this.attacks = attacks;
            moving = true;
            attackPause = 0;
            attackIndex = 0;
            speed = 0;
        }


        public override void Action(Button action, Character character, Entity entity, int index)
        {
            currentTarget = (Enemy) entity;
            if (entity.GetType() == typeof(Character))
            {
                currentPartyMemeber = (Character) entity;
            }
            base.Action(action, (Slapstick) character, currentTarget, index);
        }

        public override void Move()
        {
            position.X += speed++;

            if (position.X > destination && moving)
            {
                position.X = destination;
                moving = false;
            }
        }

        public void PerformAttack(Enemy enemy)
        {
            if (attackIndex < attacks + addAtt)
            {
                ApplyColor(paintApplied);
                attackIndex++;
                attackPause = 0.1;

                if (attackIndex == 8 + addAtt)
                {
                    attackPause = .75;
                    ApplyColor(1);
                    multiplier *= 3;
                }
                else
                {
                    Attack(enemy, power * multiplier);
                }
            }
        }

        public void RangeAttack(int attacks, Enemy enemy)
        {
            for (int i = 0; i < attacks; i++)
            {
                attackPause = 0.1;
                ApplyColor(1);
                Attack(enemy, power * multiplier);
            }
        }

        public override void Abilities(int index)
        {
            switch (index)
            {
                case 0:
                    break;
                case 1:     // Color Rush
                    stamina -= 20;
                    destination = 1000;
                    BasicAttack(3, currentTarget);
                    break;
                case 2:     // Don't Stop
                    armor = 25;
                    shield = 25;
                    break;
                case 3:     // Stop Sign Swirl
                    stamina -= 35;
                    destination = 1000;
                    BasicAttack(9, currentTarget);
                    ApplyColor(1);
                    paintApplied = 0;
                    break;
                case 4:     // Colorful High Five
                    openWindow = true;
                    break;
                case 5:     // Overcome Fears
                    regen = 40;
                    break;
                case 6:     // Paint Assault
                    rangePaint = !rangePaint;
                    break;

            }
        }
        public override void Update(GameTime gameTime)
        {
            if (Alive)
            {
                base.Update(gameTime);
                if (moving)
                {
                    Move();
                }
                else if (attackPause > 0)
                {
                    attackPause -= gameTime.ElapsedGameTime.TotalSeconds;

                }
                else
                {
                    if (position.X == 1000 && attackIndex < attacks + addAtt)
                    {
                        addAtt = 0;
                        if (rangePaint)
                        {
                            addAtt = 2;
                        }
                        PerformAttack(currentTarget);
                    }

                    else if (attackIndex >= attacks)
                    {
                        paintApplied = 1;

                        Return();
                    }
                    
                }
            }
            
        }

        public override void Draw(SpriteBatch sb, Color tint)
        {
            base.Draw(sb, tint);
        }
    }
}
