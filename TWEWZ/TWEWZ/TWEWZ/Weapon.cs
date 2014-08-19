#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
#endregion Using Statements

/// The World Ends With Zombies
/// Man & Coffee Games
/// Controls the basic functions of the weapons

namespace TWEWZ
{
    class Weapon : Item
    {
        #region Attributes
        private int weaponDamage;
        #endregion Attributes

        #region Constructor
        public Weapon(int x, int y, int width, int height, int damage, int type, SpriteBatch spriteBatch)
            : base(x, y, width, height, type, null, Rectangle.Empty, spriteBatch)
        {
            weaponDamage = damage;
        }
        #endregion Constructor

        #region Properties
        public int WeaponDamage
        {
            get { return weaponDamage; }
            set { weaponDamage = value; }
        }
        #endregion Properties
    }
}
