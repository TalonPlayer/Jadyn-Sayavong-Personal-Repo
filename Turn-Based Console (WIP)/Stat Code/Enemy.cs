using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stat_Code
{
    internal class Enemy : Character
    {
        public Enemy(string name) :
            base(name)
        {
            LoadCharacter(name);
        }

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
