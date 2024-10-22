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
    internal class Gojo_Satoru : Mage
    {
        private int cursed;
        private int blue;
        private int red;
        /* 
         * Gojo Satoru specializes in dealing magic damage using cursed energy
         * 
         * Passive: 
         * The Six Eyes
         * (Evade 80%) Gojo is able to detect most attacks coming at him, making him quickly react and dodge
         * 
         * Infinity
         * Gojo is immune to any damage if his cursed energy is above 500
         * 
         * Hollow Purple
         * Hollow Purple will become active when his Blue meter and Red meter are exactly 50/50
         * Abilities:
         * 
         * (50 S) Wing Chun Flurry - Gojo deals 25 light physical attacks
         * 
         * (200 CE) Hollow Blue - Gojo deals (Heavy) Magic damage and adds 5-15 blue to the blue meter
         * (All CE) Maximum Blue - Gojo spends his entire Cursed Energy to deal (True Magic Damage). Makes blue max
         * 
         * (400 CE) Hollow Red - Gojo deals (Heavy) Magic damage and adds 15 - 30 red to the red meter
         * 
         * (All S) Hollow Purple - Gojo launches an imaginary mass of both hollow red and hollow blue.
         * 
         * Hollow purple will deal (True Magic Damage) x (Stamina - Amount of times)
         * 
         * (Half of current S) Concentrate - Gojo's next ability does 3x more damage 
         * 
         * Domain Expansion: Unlimited Void
         * Gojo stuns all enemies for 2 turns, they also take quadruple damage from every incoming damage
         */
        public Gojo_Satoru(Texture2D asset, Rectangle position, string name, int level) :
            base(asset, position, name, level)
        {
            attacks = 3;

            health = 1000;
            maxHealth = 1000;

            phys = 30;
            magic = 100;

            armor = 0;

            shield = 0;

            cursed = 1500;
            energy = 1500;
            stamina = 1000;

            regen = 100;

            abilityNames = new List<string>
            {
                "Unlimited Void",
                "Hollow Purple",
                "Wing Chun Flurry",
                "Hollow Blue",
                "Maximum Blue",
                "Hollow Red",
                "Concentrate"
            };

            power = energy + cursed * magic;
        }
        public override void BasicAttack(int attacks, Enemy enemy)
        {
            for (int i = 0; i < attacks; i++)
            {
                Attack(enemy, power);
            }
        }

        public override void Action(Button action, Character character, Entity enemy, int index)
        {
            base.Action(action, (Gojo_Satoru) character, (Enemy) enemy, index);
        }

        public override void Move()
        {

        }


        public override void Abilities(int index)
        {
            switch (index)
            {
                case 1:
                    break;
                case 2:
                    break;
                case 3:
                    break;
                case 4:
                    break;
                case 5:
                    break;
                case 6:
                    break;
            }
        }

        public override void Draw(SpriteBatch sb, Color tint)
        {
            base.Draw(sb, tint);
        }
    }
}
