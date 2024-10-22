using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stat_Code
{
    internal class Slapstick : Character
    {
        public Slapstick(string name):
            base(name)
        { 
            LoadCharacter(name);

            abilities = new List<string>
            {
                { "Color Rush" },
                { "Stop Sign Swirl" },
                { "Paint Arrows"},
                { "Over Come Fears" },
                { "Colorful High Five" },
                { "Self Stun"}
            };
        }
        public override void DoAction(int index, List<Character> party, Character enemy)
        {
            target = (Enemy) enemy;
            switch (index)
            {
                case 0:
                    Attack(target, stats["Physical Damage"]);
                    break;
                case 1:
                    Console.WriteLine(abilities[0]);
                    break;
                case 2:
                    Console.WriteLine(abilities[1]);
                    break;
                case 3:
                    Console.WriteLine(abilities[2]);
                    break;
                case 4:
                    Console.WriteLine(abilities[3]);
                    break;
                case 5:
                    Console.WriteLine(abilities[4]);
                    Console.WriteLine(SelectMember(party) + " was chosen"!);
                    break;
                case 6:
                    stats["Stunned"] = 3;
                    disabled = true;
                    break;
            }
        }
    }
}
