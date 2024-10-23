using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stat_Code
{
    interface IEntity
    {
        public bool IsAlive { get; }
        public List<string> Abilities { get; }
        public string Name { get; }

        public void Attack(Character target, double damage);

        public void TakeDamage(double damage);

        public void Heal(Character target, double healing);
    }
}
