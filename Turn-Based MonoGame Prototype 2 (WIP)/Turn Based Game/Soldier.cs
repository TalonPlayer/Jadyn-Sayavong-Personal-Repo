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
    // Slapstick
    // Grimir Oversplitter

    // These units are attack based and will focus on melee and crits
    // They will have abilities that pertain to the character
    abstract class Soldier : Character
    {
        protected int regen;
        public Soldier(Texture2D asset, Rectangle position, string name, int level) :
            base(asset, position, name, level)
        {
            // Default Stats
            power = 25;     // Base Damage

            crit = 15;       // Base Crit Chance
            evade = 5;      // Base Evade Chance

            defense = 10;   // Base Defense

            stamina = 200;  // Base Stamina

            energy = 50;    // Base Stamina

            regen = 10;     // Regen Stamina

            ultimate = 0;

            // Soldiers are not magic based, so they will not recharge energy
        }
        public override void Action(Button action, Character character, Entity enemy, int index)
        {
            if (action.Click)
            {
                if (action.Text == "ATTACK")
                {
                    BasicAttack(1, (Enemy) enemy);
                }
                else
                {
                    // ability 0 is the ultimate ability
                    for (int i = 0; i < abilityNames.Count; i++)
                    {
                        if (action.Text == abilityNames[i])
                        {
                            Abilities(i);
                        }
                    }
                }
            }
        }
    }
}
