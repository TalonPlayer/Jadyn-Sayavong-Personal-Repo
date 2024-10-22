using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.PortableExecutable;
using System.Runtime.CompilerServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Stat_Code
{
    abstract class Character : IEntity
    {
        // Private fields, child classes can't change these.
        private string name;
        private bool isAlive;
        private int aIndex;
        private bool didAction;

        // Protected fields, child classes are allowed to change these, but main can't
        protected Random rng;

        // These are the basic stats for the character
        protected Dictionary<string, double> stats = new Dictionary<string, double>();

        // Elemental damage and weaknesses
        protected Dictionary<string, double> elements = new Dictionary<string, double>();
        protected Dictionary<string, double> resistances = new Dictionary<string, double>();

        // Character abilities
        protected List<string> abilities;

        // The character this character is targeting
        protected Character target;

        // The action
        protected string action;

        // Stunned
        protected bool disabled;

        // The only thing the character needs is the name, so that the character can be
        // polymorphed into their appropriate Child Class
        public Character(string name) 
        {
            this.name = name;

            rng = new Random();
            isAlive = true;
            disabled = false;
            action = "Attack";
            didAction = false;

            DefaultStats = new Dictionary<string, double>();
            DefaultElements = new Dictionary<string, double>();
            DefaultResistances = new Dictionary<string, double>();
        }

        // Properties so that main can access, but can't change it
        public bool DidAction { get { return didAction; } }
        public bool Disabled {  get { return disabled; } } 
        public string Action { get { return action; } }
        public string Name { get { return name; } }
        public bool IsAlive { get { return isAlive; } }
        public List<string> Abilities { get { return abilities; } }

        // The Default so that they can return to normal after boosts and reductions end
        public Dictionary<string, double> DefaultStats { get;}
        public Dictionary<string, double> DefaultElements { get; }
        public Dictionary<string, double> DefaultResistances { get; }

        /// <summary>
        /// This Character will attack the target for the amount of damage.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="damage"></param>
        public void Attack(Character target, double damage)
        {
            Console.WriteLine($"\n{Name} did {damage} damage to {target.Name}!");
            target.TakeDamage(damage);
        }
        /// <summary>
        /// This Character's health is updated after being attacked.
        /// </summary>
        /// <param name="damage"></param>
        public void TakeDamage(double damage) 
        {
            stats["Current Health"] -= damage;
            Console.WriteLine($"\n{Name} now has {stats["Current Health"]} health!");
            if (stats["Current Health"] <= 0)
            {
                isAlive = false;
            }
        }

        /// <summary>
        /// This will regenerate the targeted Character the amount of healing.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="healing"></param>
        public void Heal(Character target, double healing)
        {
            target.stats["Current Health"] += healing;
        }

        public void ChangeValue(int value, string name, string category)
        {
            switch (category) 
            {
                case "Stats":
                    stats[name] += value;
                    break;
                case "Magic Elements":
                    elements[name] += value;
                    break;
                case "Magic Resistances":
                    resistances[name] += value;
                    break;
            }

        }

        public void ResetValue(string name, string category)
        {
            switch (category) 
            {
            case "Stats":
                stats[name] = DefaultStats[name];
                break;
            case "Magic Elements":
                elements[name] = DefaultElements[name];
                break;
            case "Magic Resistances":
                resistances[name] = DefaultResistances[name];
                break;
            }
        }

        /// <summary>
        /// This Character will Regenerate health, regain Stamina and Mana, and any status counters will be subtracted.
        /// </summary>
        public virtual void TurnStats()
        {
            didAction = false;
            action = "Attack";

            // Healt this character for a percent amount
            Heal(this, stats["Current Health"] * (stats["Regeneration"] / 100));
            stats["Stamina"] += stats["Rest"];
            stats["Mana"] += stats["Think"];

            // When a stat is subtracted, it means that the amount of turns a status is active
            // goes down.
            // Ex. If Bleeding starts at 3 turns, bleeding will now be 2 on the next turn, then 1, then 0
            if (stats["Bleeding"] > 0)
            {
                // Damage the player for 25% of their current health
                stats["Current Health"] -= stats["Current Health"] * .25;
                stats["Bleeding"]--;
            }
            if (stats["Blinded"] > 0)
            {
                // The character is blinded, so they have a 90% Chance of missing
                stats["Miss Chance"] = 90;
                stats["Blinded"]--;
            }
            else
            {
                // Miss Chance is set back to normal
                stats["Miss Chance"] = DefaultStats["Miss Chance"];
            }
            if (stats["Stunned"] > 0)
            {
                // The character is disabled and it will show that the character is stunned
                action = "Stunned";
                disabled = true;
                stats["Stunned"]--;
            }
            else
            {
                // Character is not disabled
                disabled = false;
            }
            if (stats["Weakened"] > 0)
            {
                // The character's physical damage is reduced by 20%
                stats["Physical Damage"] *= .20;
                stats["Weakened"]--;
            }
            else
            {
                // Character's physical damage is set back to normal
                stats["Physical Damage"] = DefaultStats["Physical Damage"];
            }
        }

        /// <summary>
        /// A Character will run the action's method based on the index selected. 
        /// <para/> The action can target an enemy or a party member.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="party"></param>
        /// <param name="enemy"></param>
        public abstract void DoAction(int index, List<Character> party, Character enemy);

        /// <summary>
        /// Select the action of a character.
        /// </summary>
        /// <param name="party"></param>
        /// <param name="enemy"></param>
        public void SelectAction(List<Character> party, Character enemy)
        {
            Console.WriteLine(
                            $"\nWhat will {Name} do?" +
                            $"\n\t1. Attack" +
                            $"\n\t2. Ability" +
                            $"\n\t3. Stats" +
                            $"\n\t4. Do Action");
            int choice = int.Parse(Console.ReadLine());

            switch (choice)
            {
                case 1:
                    // Attack will simply set the action to attack
                    // The action index is set to 0, because basic attack is 0
                    action = "Attack";
                    aIndex = 0;
                    break;
                case 2:
                    // Starts at 1 to show that the first ability is #1 and so on
                    int iteration = 1;
                    foreach (string ability in Abilities)
                    {
                        Console.WriteLine($"{iteration}. {ability}");
                        iteration++;
                    }
                    // Get the index of the ability
                    aIndex = int.Parse(Console.ReadLine());

                    // The action string is chosen from the abilities list.
                    action = abilities[aIndex - 1];

                    break;
                case 3:
                    // Show the current stats of the character
                    Console.WriteLine(ToString());
                    break;
                case 4:
                    // Do the action.
                    DoAction(aIndex, party, enemy);
                    // The character did the action, so they can't be chosen again
                    action = "Turn is Over";
                    didAction = true;
                    break;
            }
        }

        /// <summary>
        /// Select a party member. Does not include this Character.
        /// </summary>
        /// <param name="party"></param>
        /// <returns>string (Character's Name).</returns>
        public string SelectMember(List<Character> party)
        {
            // Add every character from the party besides this character to a new list.
            int index = 0;
            List<Character> list = new List<Character>();
            foreach (Character member in party)
            {
                if (member != this)
                {
                    list.Add(member);
                }
            }

            // Print every character in the new list that exludes this Character
            for (int i = 0; i < list.Count; i++)
            {
                Console.WriteLine($"{index + 1}. {list[i].Name}");
            }
            index = int.Parse(Console.ReadLine()) - 1;

            // That character is selected
            return list[index].Name;
        }

        /// <summary>
        /// Print the current stats of a character.
        /// </summary>
        /// <returns>string (Character's Stats).</returns>
        public override string ToString()
        {
            string result = $"\n{Name}";

            result += "\nStats";
            foreach (var stat in stats)
            {
                result += $"\n{stat.Value}\t - \t{stat.Key}";
            }
            result += "\nElements";
            foreach (var element in elements)
            {
                result += $"\n{element.Value}\t - \t{element.Key}";
            }
            result += "\nResistances";
            foreach (var resistance in resistances)
            {
                result += $"\n{resistance.Value}\t - \t{resistance.Key}";
            }
            return result;
        }

        /// <summary>
        /// Load the character's stats from a .txt file.
        /// </summary>
        /// <param name="name"></param>
        public void LoadCharacter(string name)
        {
            StreamReader input = null;

            try
            {
                input = new StreamReader("../../../Units/" + name + ".txt");

                Console.WriteLine("File is being read for " + name);

                string line = null;

                while ((line = input.ReadLine()) != null)
                {
                    // Ex. 100,Current Health,Stats
                    string[] segments = line.Split(',');

                    // 100
                    int value = int.Parse(segments[0]);
                    
                    // Current Health
                    string stat = segments[1];
                    
                    // Stats
                    string category = segments[2];

                    // Separate the stats based on the category
                    if (category == "Stats")
                    {
                        stats.Add(stat, value);
                        DefaultStats.Add(stat, value);
                    }
                    else if (category == "Magic Elements")
                    {
                        elements.Add(stat, value);
                        DefaultElements.Add(stat, value);
                    }
                    else if (category == "Magic Resistances")
                    {
                        resistances.Add(stat, value);
                        DefaultResistances.Add(stat,value);
                    }
                }
            }
            catch (FileNotFoundException e)
            {
                Console.WriteLine("File not found: " + e.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine("File can not be read! : " + e.Message);
            }
            finally
            {
                if (input != null)
                {
                    input.Close();
                }
            }
        }
    }
}
