using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stat_Code
{
    internal class AssignClass
    {
        private List<Character> party = new List<Character>();
        public AssignClass(List<string> members)
        { 
            // For each member, Assign the appropriate class and add them to a new list (party)
            foreach(string member in members)
            {
                // members is the list of character names.
                party.Add(AssignClasses(member));
            }
        }

        public List<Character> Party { get { return party; } }

        /// <summary>
        /// Takes the character's name and creates a new object of the associated class.
        /// </summary>
        /// <param name="member"></param>
        /// <returns>Character (Polymorphed Character Classes)</returns>
        public Character AssignClasses(string member)
        {
            switch (member)
            {
                case "Slapstick":
                    return new Slapstick(member);
                case "Gojo Satoru":
                    return new Gojo(member);
                default:
                    return null;
            }
            
        }
    }
}
