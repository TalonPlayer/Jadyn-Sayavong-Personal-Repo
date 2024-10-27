using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
        private bool paintArrows;
        private int ofCounters;
        private bool allColorPassive;
        public Slapstick(string name):
            base(name)
        { 
            // Load Slapstick's Stats
            LoadCharacter(name);

            // The name of Slapstick's abilities
            abilities = new Dictionary<string, int>
            {
                { "Color Rush", 10},
                { "Stop Sign Swirl", 25},
                { "Paint Arrows", 30},
                { "Overcome Fears", 40},
                { "Colorful High Five", 25},
            };

            palette = new Dictionary<string, bool>
            {
                {"Red", false},         // Lava
                {"Orange", false},      // Fire
                {"Blue", false},        // Water
                {"Turquoise", false},   // Ice
                {"Green", false},       // Earth
                {"Lime", false},        // Wind
                {"Yellow", false},      // Lightning
                {"White", false},       // Light
                {"Black", false},       // Dark
                {"Pink", false},        // Mind
                {"Purple", false},      // Void
            };
            paintArrows = false;
            covered = 0;
            ofCounters = 0;
            allColorPassive = false;
        }

        public override void TurnStats()
        {
            if (covered > 0)
            {
                covered--;
                if (covered == 0)
                {
                    foreach (var color in palette)
                    {
                        if (palette[color.Key])
                        {
                            ChangeValue(10, ColorToElement(color.Key), "Magic Resistances");
                        }
                    }
                }
            }

            if (ofCounters > 0)
            {
                ofCounters--;
                if (ofCounters == 0)
                {
                    OvercomeFears("Physical Damage", -2);
                    OvercomeFears("Armor", -2);
                    OvercomeFears("Magic Resist", -2);
                    OvercomeFears("Regeneration", -2);
                    OvercomeFears("Stamina", -2);
                    OvercomeFears("Mana", -2);
                    OvercomeFears("Rest", -2);
                    OvercomeFears("Think", -2);
                }
            }

            if (stats["Ultimate Meter"] >= 100)
            {
                if (!abilities.ContainsKey("Ultimate Paint Blast"))
                {
                    abilities.Add("Ultimate Paint Blast", 100);
                }
            }

            base.TurnStats();
        }

        /// <summary>
        /// Slapstick's Actions.
        /// <para/> 0 Physical Attack: Basic Physical Attack
        /// <para/> 1 10S Color Rush: Slapstick will physical damage enemy 3 times, applying 3 colors. 
        /// <para/> 2 25S Stop Sign Swirl: Slapstick will do 8 small Physical attacks, applying 1 color, followed by 1 large Physical PhysicalAttack, applying 1 more.
        /// <para/> 3 15M Paint Arrows: After every action, Slapstick will follow it with an additional 2 Element attacks for 3 turns. 
        /// <para/> 4 30M Overcome Fears: For 3 turns, Slapstick's Damage, Armor, Magic Resist, Regeneration, Stamina and Mana regain are boosted by 200%.
        /// <para/> 5 30M Colorful High Five: Slapstick chooses a teammate to give them his Colorful Attacks passive for 3 turns.
        /// <para/> 6 100U Ultimate Paint Blast: Slapstick damages all enemies with an imaginary blast and applies every color.
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
                    PhysicalAttack(target, stats["Physical Damage"]);
                    ColorfulAttacks();
                    ImaginaryBoost();
                    break;
                case 1:
                    if (CheckEnergy("Stamina", 10))
                    {
                        ColorRush();
                    }
                    break;
                case 2:
                    if (CheckEnergy("Stamina", 25))
                    {
                        StopSignSwirl();
                    }
                    break;
                case 3:
                    paintArrows = !paintArrows;
                    break;
                case 4:
                    if (ofCounters == 0 && CheckEnergy("Mana", 30))
                    {
                        OvercomeFears("Physical Damage", 2);
                        OvercomeFears("Armor", 2);
                        OvercomeFears("Magic Resist", 2);
                        OvercomeFears("Regeneration", 2);
                        OvercomeFears("Stamina", 2);
                        OvercomeFears("Mana", 2);
                        OvercomeFears("Rest", 2);
                        OvercomeFears("Think", 2);
                    }
                    ofCounters = 3;
                    break;
                case 5:
                    if (CheckEnergy("Mana", 30))
                    {
                        Character selected = TurnHandler.SelectMember(party);
                        GiveColorfulAttacks(selected);
                        Console.WriteLine($"\n{selected.Name} was given Colorful Attacks"!);
                    }
                    break;
                case 6:
                    if (stats["Ultimate Meter"] == 100)
                    {
                        UltimatePaintBlast();
                        abilities.Remove("Ultimate Paint Blast");
                    }
                    break;
            }

            if (paintArrows)
            {
                if (CheckEnergy("Mana", 15))
                {
                    PaintArrows();
                    TurnHandler.DidAction = true;
                }
                else
                {
                    paintArrows = false;
                }
            }

            UseGrantedAbilities();

        }

        public void ImaginaryBoost()
        {
            Console.WriteLine(allColorPassive + " all color passive");
            if (allColorPassive)
            {
                MagicAttack(target, "Imaginary", stats["Physical Damage"]);
            }
        }

        /// <summary>
        /// If any of the colors are false, return false.
        /// If all colors are true, return true
        /// </summary>
        /// <returns></returns>
        public bool CheckColor()
        {
            foreach(var color in palette)
            {
                if (!palette[color.Key])
                {
                    return false;
                }
            }
            Console.WriteLine($"{target.Name} is covered in every paint!");
            return true;
        }

        /// <summary>
        /// Apply a color of paint to the enemy. 
        /// </summary>
        public void ColorfulAttacks()
        {
            int index = 0;
            bool loop = true;
            allColorPassive = false;
            do
            {
                index = rng.Next(0, palette.Count);
                if (!palette.ElementAt(index).Value)
                {
                    ApplyColor(index);
                    loop = false;
                }
                else if (CheckColor())
                {
                    allColorPassive = true;
                    loop = false;
                }
            } while (loop);
        }

        /// <summary>
        /// The color at the given index is set to true. The enemy will lose resistance associated with that color.
        /// </summary>
        /// <param name="index"></param>
        public void ApplyColor(int index)
        {
            string color = palette.ElementAt(index).Key;
            palette[color] = true;

            Console.WriteLine($"\n{target.Name} is now covered in {color} paint!");

            covered = 3;

            target.ChangeValue(-10, ColorToElement(color), "Magic Resistances");
        }

        private string ColorToElement(string color)
        {
            switch (color)
            {
                case "Red":
                    return "Lava";
                case "Orange":
                    return "Fire";
                case "Blue":
                    return "Water";
                case "Turquoise":
                    return "Ice";
                case "Green":
                    return "Earth";
                case "Lime":
                    return "Wind";
                case "Yellow":
                    return "Lightning";
                case "White":
                    return "Light";
                case "Black":
                    return "Dark";
                case "Pink":
                    return "Mind";
                case "Purple":
                    return "Void";
                default:
                    return "Not supposed to happen";
            }
        }

        private void ColorRush()
        {
            for(int i = 0; i < 3; i++)
            {
                PhysicalAttack(target, stats["Physical Damage"]);
                ColorfulAttacks();
                ImaginaryBoost();
            }
        }

        private void StopSignSwirl()
        {
            for (int i = 0; i < 8; i++)
            {
                PhysicalAttack(target, stats["Physical Damage"] * .50);
                ImaginaryBoost();
            }
            ColorfulAttacks();
            PhysicalAttack(target, stats["Physical Damage"] * 1.50);
            ColorfulAttacks();
            ImaginaryBoost();
        }

        private void PaintArrows()
        {
            int index = 0;
            string color = "";
            for (int i = 0; i < 2; i++)
            {
                index = rng.Next(0, palette.Count);
                color = palette.ElementAt(index).Key;
                color = ColorToElement(color);
                MagicAttack(target, color, stats["Magic Damage"]);
                ImaginaryBoost();
            }
        }

        private void OvercomeFears(string stat, int value)
        {
            ChangeValue(value * stats[stat], stat, "Stats");
        }
        private void GiveColorfulAttacks(Character member)
        {
            member.GiveAbility(ColorfulAttacks, 3);
        }

        private void UltimatePaintBlast()
        {
            foreach (var color in palette)
            {
                palette[color.Key] = true;
            }
            allColorPassive = true;
            MagicAttack(target, "Imaginary", 5000);
            stats["Ultimate Meter"] = 0;
        }
    }
}
