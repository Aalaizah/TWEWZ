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
/// Controls the movement and breakage of barricades

namespace TWEWZ
{
    class Wall : Obstructions
    {
        #region Constructor
        public Wall(int x, int y, int width, int height, int hits, Texture2D image, QuadTree quad, Color clor, SpriteBatch spriteBatch)
            : base(x, y, width, height, hits, image, quad, spriteBatch)
        {
            color = clor;
        }
        #endregion Constructor

        #region Draws the image to the screen
        public override void Draw()
        {
            if (IsActive)
                base.Draw();
        }
        #endregion Draws the image to the screen
    }
}
