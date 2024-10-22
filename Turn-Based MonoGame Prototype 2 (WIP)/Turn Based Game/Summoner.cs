using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Threading;

namespace Turn_Based_Game
{
    // Josuke Higashikata
    // Tohru Adachi
    // Yu Narukami

    // These units are both attack based and ability based
    // They will have abilities that pertain to the character
    abstract class Summoner : Character
    {
        protected Texture2D summonAsset;
        protected Rectangle summonPosition;
        protected string summonName;
        public Summoner(List<Texture2D> assets, List<Rectangle> positions, List<string> names, int level) :
            base(assets[0], positions[0], names[0], level)
        {

            summonAsset = assets[1];
            summonPosition = positions[1];
            summonName = names[1];
        }
        // Attack List
        public abstract void Attacks();

        // Ability List
        public abstract void Abilities();

    }
}