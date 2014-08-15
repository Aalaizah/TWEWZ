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
/// Code that controls the movement and implementation of grenades

namespace TWEWZ
{
    class Grenade : Ammo
    {
        #region Attributes
        private Vector2 startLoc, endLoc;
        private int grenadeSpeed;
        private float multi, xLen, yLen, lengthDiag, currentDist;
        private bool done;
        #endregion Attributes

        #region Constructor
        public Grenade(int x, int y, int width, int height, QuadTree quad, Texture2D t2d, SpriteBatch spriteBatch)
            : base(x, y, width, height, quad, Type.Grenade, t2d, spriteBatch)
        {
            Quad = quad; damage = 1;
            IsActive = true;
            grenadeSpeed = Constants.BULLET_SPEED / 5 * 3;
            multi = 1f; currentDist = 0f;
        }
        #endregion Constructor

        #region Properties
        /// <summary> Where the grenade started out. </summary>
        public Vector2 StartLoc
        {
            get { return startLoc; }

            set { startLoc = value; }
        }

        /// <summary> Where the grenade will stop. </summary>
        public Vector2 EndLoc
        {
            get { return endLoc; }

            set { endLoc = value; }
        }

        public float LengthDiag
        {
            get { return lengthDiag; }

            set { lengthDiag = value; }
        }

        #endregion Properties

        #region Move/Shoot
        public void Shoot(float dir, float dirOff, double var, Vector2 end)
        {
            Direction = dir + dirOff;
            // AmmoX && AmmoY start out the same as .rectangle properties, but is more accurate as grenade moves.
            ammoX = rectangleX; ammoY = rectangleY;
            
            // ammoX && ammoY keep track of the bullets coordinates BEFORE they are distorted by the cast to int.
            ammoX += Math.Cos(Direction) * var;
            rectangleX = (int)ammoX;
            rectangleX -= (Width / 2);
            ammoY += Math.Sin(Direction) * var;
            rectangleY = (int)ammoY;
            rectangleY -= (Height / 2);

            endLoc = end;
            startLoc = new Vector2((float)ammoX, (float)ammoY);
            xLen = endLoc.X - startLoc.X;
            yLen = endLoc.Y - startLoc.Y;
            lengthDiag = Vector2.Distance(startLoc, endLoc);
            currentDist = DiaginalDist(Center.X - startLoc.X, Center.Y - startLoc.Y);

            Direction -= dirOff;
        }

        public override void Move()
        {
            // ammoX && ammoY keep track of the bullets coordinates BEFORE they are distorted by the cast to int.
            ammoX += Math.Cos(Direction) * grenadeSpeed; rectangleX = (int)ammoX;
            ammoY += Math.Sin(Direction) * grenadeSpeed; rectangleY = (int)ammoY;
            currentDist = DiaginalDist(Center.X - startLoc.X, Center.Y - startLoc.Y);
            
            if (currentDist < lengthDiag / 2){
                multi = 1 + (currentDist / (lengthDiag / 2));
            }
            else{
                multi = ((lengthDiag - (currentDist - (lengthDiag / 2))) / (lengthDiag / 2));
            }
        }

        public bool CheckIfDone()
        {
            done = false;
            if (currentDist >= lengthDiag) { done = true; IsActive = false; }
            return done;
        }
        #endregion Move/Shoot

        #region Draws the image to the screen
        public override void Draw()
        {
            if (IsActive)
                spriteBatch.Draw(Texture, Center, CutOut, Color.White, 0, new Vector2(Texture.Width / 2, Texture.Height / 2), multi, SpriteEffects.None, 0);
        }
        #endregion Draws the image to the screen
    }
}
