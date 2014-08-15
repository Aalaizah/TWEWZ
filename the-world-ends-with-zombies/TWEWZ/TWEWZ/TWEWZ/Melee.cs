#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
/// Code for the implementation of melee type weapons

namespace TWEWZ
{
    class Melee : Weapon
    {
        #region Constructor
        public Melee(int x, int y, int width, int height, int damage, SpriteBatch spriteBatch)
            : base(x, y, width, height, damage, 1, spriteBatch)
        {
            WeaponDamage = damage;
        }
        #endregion Constructor

        #region Attack
        public void Attack(Vector2 plyrCntr, float dir, int var)
        {
            Center = plyrCntr;
            Direction = dir;

            rectangleX += (int)(Math.Cos(Direction) * (Constants.MELEE_VAR + var));
            rectangleY += (int)(Math.Sin(Direction) * (Constants.MELEE_VAR + var));

            IsActive = true;
        }
        #endregion Attack
    }
}
