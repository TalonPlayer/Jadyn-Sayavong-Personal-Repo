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
    abstract class Transformation : Character
    {
        protected Texture2D transformAsset;
        public Transformation(List<Texture2D> assets, Rectangle position, string name, int level) :
            base(assets[0], position, name, level)
        {
            transformAsset = assets[1];

        }
        // Attack List
        public abstract void Attacks();

        // Ability List
        public abstract void Abilities();
    }
}
