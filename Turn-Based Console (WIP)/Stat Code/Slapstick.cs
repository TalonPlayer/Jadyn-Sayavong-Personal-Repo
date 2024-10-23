using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Stat_Code
{
    internal class Slapstick : Character
    {
        private Dictionary<string, bool> palette;

        // The turns the enemy will be covered in paint
        private int covered;
        public Slapstick(string name):
            base(name)
        { 
            // Load Slapstick's Stats
            LoadCharacter(name);

            // The name of Slapstick's abilities
            abilities = new List<string>
            {
                { "Color Rush" },
                { "Stop Sign Swirl" },
                { "Paint Arrows"},
                { "Overcome Fears" },
                { "Colorful High Five" },
                { "Self Stun"}
            };

            palette = new Dictionary<string, bool>
            {
                {"Red", false},         // Lava
                {"Orange", false},      // Fire
                {"Blue", false},        // Water
                {"Turquoise", false},   // Ice
                {"Green", false},        // Earth
                {"Lime", false},       // Wind
                {"Yellow", false},      // Lightning
                {"White", false},       // Light
                {"Black", false},       // Dark
                {"Pink", false},        // Mind
                {"Purple", false},      // Void
            };

            covered = 0;
        }

        /// <summary>
        /// Slapstick's Actions.
        /// <para/> 0 Attack: Basic Attack
        /// <para/> 1 Color Rush: Slapstick will Physical attack the enemy 3 times, applying 3 colors. 
        /// <para/> 2 Stop Sign Swirl: Slapstick will do 8 small Physical attacks, applying 1 color, followed by 1 large Physical attack, applying 1 more.
        /// <para/> 3 Paint Arrows (Toggle): After every action, Slapstick will follow it with an additional 2 Element attacks. 
        /// <para/> 4 Overcome Fears: For 3 turns, Slapstick's Damage, Regeneration, Stamina and Mana regain are boosted by 200%.
        /// <para/> 5 Colorful High Five: Slapstick chooses a teammate to give them his Colorful Attacks passive for 3 turns.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="party"></param>
        /// <param name="target"></param>
        public override void DoAction(int index, List<Character> party, Character target)
        {
            this.target = target;
            switch (index)
            {
                case 0:
                    Attack(target, stats["Physical Damage"]);
                    ColorfulAttacks(target);
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
                    Console.WriteLine(TurnHandler.SelectMember(party) + " was chosen"!);
                    break;
                case 6:
                    stats["Stunned"] = 3;
                    disabled = true;
                    break;
            }
        }

        /// <summary>
        /// Apply a color of paint to the enemy. 
        /// </summary>
        public void ColorfulAttacks(Character enemy)
        {
            int index = 0;
            bool loop = true;
            do
            {
                index = rng.Next(0, palette.Count + 1);
                if (!palette.ElementAt(index).Value)
                {
                    ApplyColor(index, enemy);
                    loop = false;
                }
            } while (loop);
        }

        /// <summary>
        /// The color at the given index is set to true. The enemy will lose resistance associated with that color.
        /// </summary>
        /// <param name="index"></param>
        public void ApplyColor(int index, Character enemy)
        {
            string color = palette.ElementAt(index).Key;
            palette[color] = true;

            Console.WriteLine($"\n{enemy.Name} is now covered in {color} paint!");

            covered = 3;

            switch (color) 
            {
                case "Red":
                    color = "Lava";
                    break;
                case "Orange":
                    color = "Fire";
                    break;
                case "Blue":
                    color = "Water";
                    break;
                case "Turquoise":
                    color = "Ice";
                    break;
                case "Green":
                    color = "Earth";
                    break;
                case "Lime":
                    color = "Wind";
                    break;
                case "Yellow":
                    color = "Lightning";
                    break;
                case "White":
                    color = "Light";
                    break;
                case "Black":
                    color = "Dark";
                    break;
                case "Pink":
                    color = "Mind";
                    break;
                case "Purple":
                    color = "Void";
                    break;
            }
            enemy.ChangeValue(-10, color, "Magic Resistances");
        }
    }
}
