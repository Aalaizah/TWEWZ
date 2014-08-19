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
/// Code that deals with the creation of drop items

namespace TWEWZ
{
    class Item : GameObject
    {
        #region Attributes
        private bool onScreen, inInventory;
        private int itemType; // 1 - max Weapons are weapons and -1 is a food item, -2 is ammo, and -3 is a blank.
        private Random rng;
        #endregion Attributes

        #region Constructor
        public Item(int x, int y, int width, int height, int itmType, Texture2D image, Rectangle cutOut, SpriteBatch spriteBatch)
            : base(image, x, y, width, height, spriteBatch)
        {
            onScreen = true;
            rng = new Random();
            itemType = itmType;
            CutOut = cutOut;
        }
        #endregion Constructor

        #region Properties
        public bool OnScreen
        {
            get { return onScreen; }

            set { onScreen = value; }
        }

        public int ItemType
        {
            get { return itemType; }
        }

        public bool InInventory
        {
            get { return inInventory; }

            set { inInventory = value; }
        }
        #endregion Properties

        #region Methods
        public bool CheckCollision(GameObject GO)
        {
            if(onScreen){
                if (Intersects(GO))
                    return true;
                return false;
            }
            else
                return false;
        }
        #endregion Methods

        #region Draws the image to the screen
        public override void Draw()
        {
            if(onScreen)
                base.Draw();
        }
        #endregion Draws the image to the screen
    }
}
