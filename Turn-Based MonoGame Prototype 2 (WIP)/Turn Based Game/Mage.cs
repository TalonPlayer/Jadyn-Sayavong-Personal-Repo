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
    // Gojo Satoru
    // Osamu Dazai
    // Yuta Okkotsu

    // These units are ability based and will focus magic damage
    // They will have abilities that pertain to the character
    abstract class Mage : Character
    {
        protected int regen;
        protected int calm;

        protected bool concentrate;
        public Mage(Texture2D asset, Rectangle position, string name, int level) :
            base(asset, position, name, level)
        {
            power = 25;

            crit = 5;
            evade = 5;

            defense = 10;

            stamina = 100;
            energy = 200;

            ultimate = 0;

            concentrate = false;
        }
        public override void Action(Button action, Character character, Entity enemy, int index)
        {
            if (action.Click)
            {
                if (action.Text == "ATTACK")
                {
                    BasicAttack(attacks, (Enemy) enemy);
                }
            }
        }
    }
}