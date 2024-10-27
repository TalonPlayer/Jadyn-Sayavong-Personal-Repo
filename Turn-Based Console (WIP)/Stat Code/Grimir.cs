using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stat_Code
{
    internal class Grimir : Character
    {
        // Grimir's attacks applies a stack battle rage. Every stack of rage increases his damage.
        public Grimir(string name) :
            base(name)
        {
            LoadCharacter(name);

            abilities = new Dictionary<string, int>
            {
                { "Unlimited Void" , 0},
                { "Hollow Purple", 0 },
                { "Hollow Red", 0 },
                { "Hollow Blue", 0 },
                { "Maximum Output Blue", 0 },
            };
        }

        /// <summary>
        /// <para/> 0 Basic Attack: Basic Physical Attack
        /// <para/> 1 15S Axe Slam: Grimir will do large physical damage against an enemy.
        /// <para/> 2 25S Swinging Axe: Grimir will do 2 physical damage to an enemy.
        /// <para/> 3 20M Dwarven Battle Cry: Every party member is healed by Grimir and he also gives them 30% damage boost for 3 turns.
        /// <para/> 4 0 Insult: Grimir will taunt an enemy and gain 30% more armor
        /// <para/> 5 100U 
        /// <para/>
        /// <para/>
        /// </summary>
        /// <param name="index"></param>
        /// <param name="party"></param>
        /// <param name="member"></param>
        public override void DoAction(int index, List<Character> party, Character member)
        {
            target = member;
            switch (index)
            {
                case 0:
                    PhysicalAttack(target, stats["Physical Damage"]);
                    break;
                case 1:
                    Console.WriteLine(abilities.ElementAt(0).Key);
                    break;
            }
        }
    }
}
