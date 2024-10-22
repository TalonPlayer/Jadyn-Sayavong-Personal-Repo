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
                    Attack(target, stats["Physical Damage"]);
                    break;
                case 1:
                    Console.WriteLine(abilities[1]);
                    break;
                case 2:
                    Console.WriteLine(abilities[2]);
                    break;
                case 3:
                    Console.WriteLine(abilities[3]);
                    break;
                case 4:
                    Console.WriteLine(abilities[4]);
                    break;
            }
        }
    }
}
