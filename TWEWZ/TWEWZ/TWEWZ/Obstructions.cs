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
/// Parent class to deal with barricades and traps

namespace TWEWZ
{
    public class Obstructions : GameObject
    {
        #region Attributes
        private int hitsTillBroken;
        #endregion Attributes

        #region Constructor
        public Obstructions(int x, int y, int width, int height, int hits, Texture2D image, QuadTree quad, SpriteBatch spriteBatch)
            : base(image, x, y, width, height, spriteBatch)
        {
            hitsTillBroken = hits;
            IsActive = true;
            Quad = quad;
        }
        #endregion Constructor

        #region Properties
        public int HitsTillBroken
        {
            get { return hitsTillBroken; }

            set { hitsTillBroken = value; }
        }
        #endregion Properties

        #region Methods
        public void Attack()
        {
            hitsTillBroken--;
            if(hitsTillBroken <= 0){
                IsActive = false;
                Quad.RemoveObject(this);
            }
        }

        public bool CheckCollision(GameObject GO)
        {
            if(IsActive){
                if (Intersects(GO))
                    return true;
                return false;
            }
            else
                return false;
        }
        #endregion Methods
    }
}
