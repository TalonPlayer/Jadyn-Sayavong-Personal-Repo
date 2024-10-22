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
            foreach(string member in members)
            {
                party.Add(AssignClasses(member));
            }
        }

        public List<Character> Party { get { return party; } }

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
