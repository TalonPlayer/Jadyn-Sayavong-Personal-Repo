using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using static System.Collections.Specialized.BitVector32;

namespace Stat_Code
{
    internal class TurnHandler
    {
        private Character member;
        public TurnHandler(Character member) 
        { 
            this.member = member;
        }

        public string Action { get; set; }
        public int AbilityIndex { get; set; }
        public bool DidAction {  get; set; }

        /// <summary>
        /// Print the actions.
        /// </summary>
        /// <returns></returns>
        private int PrintActions()
        {
            Console.WriteLine(
                            $"\nWhat will {member.Name} do?" +
                            $"\n\t1. Attack" +
                            $"\n\t2. Ability" +
                            $"\n\t3. member.stats" +
                            $"\n\t4. Do Action");
         
            return int.Parse(Console.ReadLine());
        }

        /// <summary>
        /// Select the action of a character.
        /// </summary>
        /// <param name="party"></param>
        /// <param name="enemy"></param>
        public void SelectAction(List<Character> party, Character enemy)
        {
            switch (PrintActions())
            {
                case 1:
                    // Attack will simply set the action to attack
                    // The action index is set to 0, because basic attack is 0
                    Action = "Attack";
                    AbilityIndex = 0;
                    break;
                case 2:
                    // Starts at 1 to show that the first ability is #1 and so on
                    int iteration = 1;
                    foreach (string ability in member.Abilities)
                    {
                        Console.WriteLine($"{iteration}. {ability}");
                        iteration++;
                    }
                    // Get the index of the ability
                    AbilityIndex = int.Parse(Console.ReadLine());

                    // The action string is chosen from the abilities list.
                    Action = member.Abilities[AbilityIndex - 1];

                    break;
                case 3:
                    // Show the current member.stats of the character
                    Console.WriteLine(ToString());
                    break;
                case 4:
                    // Do the action.
                    member.DoAction(AbilityIndex, party, enemy);
                    // The character did the action, so they can't be chosen again
                    Action = "Turn is Over";
                    DidAction = true;
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
            foreach (Character m in party)
            {
                if (m != member)
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
    }
}
