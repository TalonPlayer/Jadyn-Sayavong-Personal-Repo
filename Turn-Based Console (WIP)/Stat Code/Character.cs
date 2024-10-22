using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Stat_Code
{
    abstract class Character : IEntity
    {
        private string name;
        private bool isAlive;
        protected Dictionary<string, double> stats = new Dictionary<string, double>();
        protected Dictionary<string, double> defaultStats = new Dictionary<string, double>();
        protected Dictionary<string, double> elements = new Dictionary<string, double>();
        protected Dictionary<string, double> resistances = new Dictionary<string, double>();
        private Random rng;
        protected List<string> abilities;
        protected Character target;
        protected string action;
        private int aIndex;
        private bool didAction;
        protected bool disabled;
        public Character(string name) 
        {
            this.name = name;

            rng = new Random();
            isAlive = true;
            disabled = false;
            action = "Attack";
            didAction = false;
        }
        public bool DidAction { get { return didAction; } }
        public bool Disabled {  get { return disabled; } } 

        public string Action { get { return action; } }
        public string Name { get { return name; } }

        public bool IsAlive { get { return isAlive; } }

        public List<string> Abilities { get { return abilities; } }

        public Dictionary<string, double> Stats { get { return stats; } }

        public void Attack(Character target, double damage)
        {
            Console.WriteLine($"\n{Name} did {damage} damage to {target.Name}!");
            target.TakeDamage(damage);
        }
        public void TakeDamage(double damage) 
        {
            stats["Current Health"] -= damage;
            Console.WriteLine($"\n{Name} now has {Stats["Current Health"]} health!");
            if (stats["Current Health"] <= 0)
            {
                isAlive = false;
            }
        }

        public void Heal(Character target, double healing)
        {
            target.stats["Current Health"] += healing;
        }

        public virtual void TurnStats()
        {
            didAction = false;
            action = "Attack";

            stats["Current Health"] += stats["Current Health"] * (stats["Regeneration"] / 100);
            stats["Stamina"] += stats["Rest"];
            stats["Mana"] += stats["Think"];

            if (stats["Bleeding"] > 0)
            {
                stats["Current Health"] -= stats["Current Health"] * .25;
                stats["Bleeding"]--;
            }
            if (stats["Blinded"] > 0)
            {
                stats["Blinded"]--;
                stats["Miss Chance"] = 90;
            }
            else
            {
                stats["Miss Chance"] = defaultStats["Miss Chance"];
            }
            if (stats["Stunned"] > 0)
            {
                stats["Stunned"]--;
                action = "Stunned";
                disabled = true;
            }
            else
            {
                disabled = false;
            }
            if (stats["Weakened"] > 0)
            {
                stats["Physical Damage"] /= 20;
                stats["Weakened"]--;
            }
            else
            {
                stats["Physical Damage"] = defaultStats["Physical Damage"];
            }
        }

        public abstract void DoAction(int index, List<Character> party, Character enemy);

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
                    action = "Attack";
                    break;
                case 2:
                    int iteration = 1;
                    foreach (string ability in Abilities)
                    {
                        Console.WriteLine($"{iteration}. {ability}");
                        iteration++;
                    }
                    aIndex = int.Parse(Console.ReadLine());
                    action = abilities[aIndex - 1];

                    break;
                case 3:
                    Console.WriteLine(ToString());
                    break;
                case 4:
                    DoAction(aIndex, party, enemy);
                    action = "Turn is Over";
                    didAction = true;
                    break;
            }
        }

        public string SelectMember(List<Character> party)
        {
            int index = 0;
            List<Character> list = new List<Character>();
            foreach (Character member in party)
            {
                if (member != this)
                {
                    list.Add(member);
                }
            }
            for (int i = 0; i < list.Count; i++)
            {
                Console.WriteLine($"{index + 1}. {list[i].Name}");
            }
            index = int.Parse(Console.ReadLine()) - 1;

            return list[index].Name;
        }
        public override string ToString()
        {
            string result = $"\n{Name}";

            result += "\nStats";
            foreach (var stat in Stats)
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
                    string[] segments = line.Split(',');
                    int value = int.Parse(segments[0]);
                    string stat = segments[1];
                    string category = segments[2];

                    if (category == "Stats")
                    {
                        stats.Add(stat, value);
                        defaultStats.Add(stat, value);
                    }
                    else if (category == "Magic Elements")
                    {
                        elements.Add(stat, value);
                        defaultStats.Add(stat, value);
                    }
                    else if (category == "Magic Resistances")
                    {
                        resistances.Add(stat, value);
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
