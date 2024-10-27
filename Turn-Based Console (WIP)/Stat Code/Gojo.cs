using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stat_Code
{
    internal class Gojo : Character
    {
        public Gojo(string name) :
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
        public override void DoAction(int index, List<Character> party, Character enemy)
        {
            target = (Enemy)enemy;
            switch (index)
            {
                case 0:
                    PhysicalAttack(target, stats["Physical Damage"]);
                    break;
                case 1:
                    Console.WriteLine(abilities.ElementAt(0).Key);
                    break;
                case 2:
                    Console.WriteLine(abilities.ElementAt(1).Key);
                    break;
                case 3:
                    Console.WriteLine(abilities.ElementAt(2).Key);
                    break;
                case 4:
                    Console.WriteLine(abilities.ElementAt(3).Key);
                    break;
                case 5:
                    Console.WriteLine(abilities.ElementAt(4).Key);
                    break;
            }

            UseGrantedAbilities();
        }
    }
}
