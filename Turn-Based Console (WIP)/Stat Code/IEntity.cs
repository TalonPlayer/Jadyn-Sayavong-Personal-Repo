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
        public Dictionary<string, int> Abilities { get; }
        public string Name { get; }

        public void PhysicalAttack(Character target, double damage);

        public void TakeDamage(double damage);

        public void Heal(Character target, double healing);
    }
}
