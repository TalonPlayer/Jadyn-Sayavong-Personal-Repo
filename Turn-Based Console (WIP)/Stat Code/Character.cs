using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.PortableExecutable;
using System.Runtime.CompilerServices;
using System.Security;
using System.Security.Cryptography.X509Certificates;
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
        private bool didAction;
        private TurnHandler turnHandler;

        // Protected fields, child classes are allowed to change these, but main can't
        protected Random rng;

        // These are the basic stats for the character
        protected Dictionary<string, double> stats = new Dictionary<string, double>();

        // Elemental damage and weaknesses
        protected Dictionary<string, double> elements = new Dictionary<string, double>();
        protected Dictionary<string, double> resistances = new Dictionary<string, double>();

        // Character abilities
        protected Dictionary<string, int> abilities = new Dictionary<string, int>();

        // The character this character is targeting
        protected Character target;

        // The action
        protected string action;

        // Stunned
        protected bool disabled;

        // Granted abilities from other characters
        private Dictionary<Action, int> grantedAbilities = new Dictionary<Action, int>();

        // The only thing the character needs is the name, so that the character can be
        // polymorphed into their appropriate Child Class
        public Character(string name) 
        {
            this.name = name;

            rng = new Random();
            isAlive = true;
            disabled = false;
            didAction = false;

            DefaultStats = new Dictionary<string, double>();
            DefaultElements = new Dictionary<string, double>();
            DefaultResistances = new Dictionary<string, double>();

            turnHandler = new TurnHandler(this);
            TurnHandler.Action = "Attack";

        }

        // Properties so that main can access, but can't change it
        public bool DidAction { get { return TurnHandler.DidAction; } }
        public bool Disabled {  get { return disabled; } } 
        public string Action { get { return TurnHandler.Action; } }
        public string Name { get { return name; } }
        public bool IsAlive { get { return isAlive; } }
        public Dictionary<string, int> Abilities { get { return abilities; } }

        // The Default so that they can return to normal after boosts and reductions end
        public Dictionary<string, double> DefaultStats { get;}
        public Dictionary<string, double> DefaultElements { get; }
        public Dictionary<string, double> DefaultResistances { get; }
        public TurnHandler TurnHandler { get { return turnHandler; } }

        /// <summary>
        /// This Character will attack the target for the amount of damage.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="damage"></param>
        public void PhysicalAttack(Character target, double damage)
        {
            Console.WriteLine($"\n{Name} did {damage} damage to {target.Name}!");
            double reduce = target.stats["Armor"] * (stats["Lethality"] / 100);
            reduce = (target.stats["Armor"] - reduce) / 100;
            reduce = 1 - reduce;
            damage = damage * reduce;
            target.TakeDamage(damage);
            damage *= stats["Ultimate Rate"] / 100;
            stats["Ultimate Meter"] += damage;
            Console.WriteLine(stats["Ultimate Meter"]);
        }

        public void MagicAttack(Character target, string element, double damage)
        {
            Console.WriteLine($"\n{Name} did {damage} ({elements[element]} boost) {element} damage.");
            Console.WriteLine($"\n{target.Name} has {target.stats["Magic Resist"]} ({target.resistances[element]} boost) {element} resistance.");

            double reduce = target.stats["Magic Resist"] * (stats["Spell Penetration"] / 100);
            reduce = (target.stats["Magic Resist"] - reduce) / 100;
            reduce = 1 - reduce;
            damage = damage + (damage * (resistances[element] - elements[element] / 100));
            damage = damage * reduce;
            target.TakeDamage(damage);
            damage *= stats["Ultimate Rate"] / 100;
            stats["Ultimate Meter"] += damage;
            Console.WriteLine(stats["Ultimate Meter"]);
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
            Console.WriteLine($"{target.stats["Current Health"]}");
            target.stats["Current Health"] += healing;
            Console.WriteLine($"{target.Name} was healed for {healing} health! ({target.stats["Current Health"]})");
        }

        public bool CheckEnergy(string energy, int value)
        {
            if (stats[energy] < value)
            {
                Console.WriteLine($"{Name} does not have enough {energy} for this!");
                TurnHandler.DidAction = false;
                TurnHandler.Action = "Attack";
                TurnHandler.AbilityIndex = 0;
                return false;
            }
            stats[energy] -= value;
            return true;
        }
        public void ChangeValue(double value, string name, string category)
        {
            switch (category) 
            {
                case "Stats":
                    stats[name] = value;
                    break;
                case "Magic Elements":
                    elements[name] = value;
                    break;
                case "Magic Resistances":
                    resistances[name] = value;
                    break;
            }

        }
        /// <summary>
        /// This Character will Regenerate health, regain Stamina and Mana, and any status counters will be subtracted.
        /// </summary>
        public virtual void TurnStats()
        {
            turnHandler.DidAction = false;
            turnHandler.Action = "Attack";
            turnHandler.AbilityIndex = 0;

            stats["Ultimate Meter"] = Math.Min(100, stats["Ultimate Meter"]);
            AbilityCounter();
            // Healt this character for a percent amount
            Heal(this, stats["Current Health"] * (stats["Regeneration"] / 100));
            stats["Stamina"] += stats["Rest"];
            stats["Mana"] += stats["Think"];

            // When a stat is subtracted, it means that the amount of turns a status is active
            // goes down.
            // Ex. If Bleeding starts at 3 turns, bleeding will now be 2 on the next turn, then 1, then 0
            if (stats["Bleeding"] > 0)
            {
                stats["Bleeding"]--;
                // Damage the player for 25% of their current health
                stats["Current Health"] -= stats["Current Health"] * .25;
            }
            if (stats["Blinded"] > 0)
            {
                // The character is blinded, so they have a 90% Chance of missing
                stats["Miss Chance"] = 90;
                stats["Blinded"]--;
                if (stats["Blinded"] == 0)
                {
                    // Miss Chance is set back to normal
                    stats["Miss Chance"] = DefaultStats["Miss Chance"];
                }
            }
            if (stats["Stunned"] > 0)
            {
                // The character is disabled and it will show that the character is stunned
                action = "Stunned";
                disabled = true;
                stats["Stunned"]--;
                if (stats["Stunned"] == 0)
                {
                    disabled = false;
                }
            }
            if (stats["Weakened"] > 0)
            {
                // The character's physical damage is reduced by 20%
                stats["Physical Damage"] *= .20;
                stats["Weakened"]--;
                if (stats["Weakened"] == 0)
                {
                    stats["Physical Damage"] = DefaultStats["Physical Damage"];
                }
            }
        }

        public void UseGrantedAbilities()
        {
            foreach (var ability in grantedAbilities)
            {
                ability.Key.Invoke();
            }
        }

        public void GiveAbility(Action ability, int turns)
        {
            grantedAbilities.Add(ability, turns);
        }

        public void AbilityCounter()
        {
            if (grantedAbilities != null)
            {
                foreach (var abilities in grantedAbilities)
                {
                    grantedAbilities[abilities.Key]--;
                    if (grantedAbilities[abilities.Key] == 0)
                    {
                        grantedAbilities.Remove(abilities.Key);

                        Console.WriteLine($"{abilities.Key} was removed!");
                    }
                }
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
