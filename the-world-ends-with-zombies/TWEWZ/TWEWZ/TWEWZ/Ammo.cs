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
/// Code that controls the movement and implementation of bullets

namespace TWEWZ
{
    class Ammo : Item
    {
        #region Attributes
        protected int damage;
        protected double ammoX, ammoY, prevX, prevY;
        protected Type ammoType;

        public enum Type
        {
            Bullet, Grenade, Rocket
        }
        #endregion Attributes

        #region Constructor
        public Ammo(int x, int y, int width, int height, QuadTree quad, Type type, Texture2D t2d, SpriteBatch spriteBatch)
            : base(x, y, width, height, 1, t2d, new Rectangle(0, 0, t2d.Width, t2d.Height), spriteBatch)
        {
            Quad = quad;
            damage = 1;
            AmmoType = type;
            IsActive = false;
        }
        #endregion Constructor

        #region Properties
        public double AmmoX
        {
            get { return ammoX; }
            set { ammoX = value; }
        }

        public double AmmoY
        {
            get { return ammoY; }
            set { ammoY = value; }
        }

        public int Damage
        {
            get { return damage; }
            set { damage = value; }
        }

        public double PrevX
        {
            get { return prevX; }
            set { prevX = value; }
        }

        public double PrevY
        {
            get { return prevY; }
            set { prevY = value; }
        }

        /// <summary>
        /// Overrides GO.IsActive. Also changes AmmoType back to Bullet when inactive.
        /// </summary>
        public new bool IsActive
        {
            get { return base.IsActive; }

            set { base.IsActive = value; if (!value && ammoType != Type.Bullet) { AmmoType = Type.Bullet; } }
        }

        /// <summary> When a new type is set, also changes the texture CutOut and GO Width/Height </summary>
        public Type AmmoType
        {
            get { return ammoType; }

            set
            {
                ammoType = value;
                if (ammoType == Type.Bullet)
                {
                    CutOut = new Rectangle(168, 84, 42, 42);
                    Width = Constants.AMMO_SIZE_X;
                    Height = Constants.AMMO_SIZE_Y;
                }
                else if (ammoType == Type.Rocket)
                {
                    CutOut = new Rectangle(42, 42, 42, 42);
                    Width = Constants.GRENADE_SIZE_X;
                    Height = Constants.GRENADE_SIZE_Y;
                }
                else if (ammoType == Type.Grenade)
                {
                    CutOut = new Rectangle(0, 126, 42, 42);
                    Width = Constants.ROCKET_SIZE_X;
                    Height = Constants.ROCKET_SIZE_Y;
                }
            }
        }
        #endregion Properties

        #region Shoot and Move
        public void Shoot(Vector2 playerCenter, int damge, float dir, float dirOff, double var)
        {
            Direction = dir + dirOff;
            damage = damge;
            // AmmoX && AmmoY start out the same as .rectangle properties, but is more accurate as bullet moves.
            ammoX = prevX = rectangleX = (int)playerCenter.X;
            ammoY = prevY = rectangleY = (int)playerCenter.Y;

            // ammoX && ammoY keep track of the bullets coordinates BEFORE they are distorted by the cast to int.
            ammoX += Math.Cos(Direction) * var;
            rectangleX = (int)ammoX;
            rectangleX -= (Width / 2);
            ammoY += Math.Sin(Direction) * var;
            rectangleY = (int)ammoY;
            rectangleY -= (Height / 2);

            Direction -= dirOff;
        }

        public virtual void Move()
        {
            prevX = ammoX; prevY = ammoY;
            // ammoX && ammoY keep track of the bullets coordinates BEFORE they are distorted by the cast to int.
            ammoX += Math.Cos(Direction) * Constants.BULLET_SPEED;
            rectangleX = (int)ammoX;
            ammoY += Math.Sin(Direction) * Constants.BULLET_SPEED; ;
            rectangleY = (int)ammoY;
        }
        #endregion Shoot and Move

        #region Draws the image to the screen
        public override void Draw()
        {
            if (IsActive)
            {
                base.Draw();
            }
        }
        #endregion Draws the image to the screen
    }
}
