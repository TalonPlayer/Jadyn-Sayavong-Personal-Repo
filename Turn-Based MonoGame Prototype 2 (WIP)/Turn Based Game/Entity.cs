using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Security.Cryptography.X509Certificates;
using System.Diagnostics;

namespace Turn_Based_Game
{
    abstract class Entity
    {
        protected int level;
        protected int health;

        protected int maxHealth;

        protected int power;    // Base Power Multiplier

        protected int magic;    // Magic Damage
        protected int phys;     // Physical Damage

        protected int crit;     // Percent Chance to deal double damage with physical
        protected int evade;    // Percent Chance to dodge any attack

        protected int defense;  // Base Defense Multiplier
        protected int armor;    // Defense against physical attacks
        protected int shield;   // Defense against magic attacks

        protected int stamina;  // Stat used for Physical attacks
        protected int energy;   // Stat used for Magic Attacks

        protected int ultimate;

        private bool alive;

        private Texture2D asset;

        private string name;

        protected Rectangle position;

        protected Dictionary<Button, Rectangle> abilitiesList;

        protected Dictionary<Button, Rectangle> itemsList;

        protected Enemy currentTarget;

        protected Character currentPartyMemeber;

        protected Random rng = new Random();

        public Entity(Texture2D asset, Rectangle position, string name, int level) 
        {
            this.asset = asset;
            this.position = position;
            this.name = name;
            this.level = level;
            
            health = 50 * level;
            power = 120 + (5 * level);
            defense = 0;
            stamina = 100;


            alive = true;
        }

        public Texture2D Asset { get { return asset; } }   
        public Rectangle Position { get { return position; } }
        
        public string Name { get { return name; } }

        public bool Alive {  get { return alive; } }

        public int Power { get { return power; } }
        public int Health {  get { return health; } }
        public int MaxHealth {  get { return maxHealth; } }


        public Dictionary<Button, Rectangle> AbilityList
        {
            get { return abilitiesList; }
        }
        public Dictionary<Button, Rectangle> ItemList
        {
            get { return itemsList; }
        }

        // Player attacks the enemy

        // Player is attacker
            // Attack the target

        // Enemy is target
            // Take damage from the attacker

        public abstract void Update(GameTime gameTime);

        public virtual void Draw(SpriteBatch sb, Color tint)
        {
            if (Alive)
            {
                sb.Draw(asset, Position, tint);
            }
        }


        public virtual void TakeDamage(int damage)
        {
            health -= damage;

            alive = health > 0;
        }

        public virtual void Attack(Enemy enemy, int damage)
        {
            if (rng.Next(0, 101) <= crit)
            {
                damage *= 2;
                Debug.WriteLine("Critical!");
            }
            enemy.TakeDamage(damage);
        }

        public abstract void Action(Button action, Character character, Entity enemy, int index);

        public abstract void BasicAttack(int attacks, Enemy enemy);

        public abstract void Move();

        public abstract void Abilities(int index);
    }
}
