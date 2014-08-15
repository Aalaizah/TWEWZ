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
/// Adds the functions of a firearm to the "Weapon" class

namespace TWEWZ
{
    class Gun : Weapon
    {
        #region Attributes
        private int fireRate;
        #endregion Attributes

        #region Constructor
        public Gun(int x, int y, int width, int height, int damage, int type, SpriteBatch spriteBatch)
            : base(x, y, width, height, damage, type, spriteBatch)
        {
            WeaponDamage = damage;
        }
        #endregion Constructor

        #region Properties
        // Alters timer for firing!
        public int FireRate
        {
            get { return fireRate; }

            set { fireRate = value; }
        }
        #endregion Properties

        #region Methods
        public void Fire(Ammo ammo, Vector2 playerCenter, float dir, float dirOff, double var)
        {
            ammo.IsActive = true;
            ammo.Shoot(playerCenter, WeaponDamage, dir, dirOff, var);
        }
        #endregion Methods
    }
}
