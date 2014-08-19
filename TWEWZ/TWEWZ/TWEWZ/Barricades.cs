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
    public class Barricade : Obstructions
    {
        #region Attributes
        private double tempX, tempY;
        private bool beingHeld, under;
        #endregion Attributes

        #region Constructor
        public Barricade(int x, int y, int width, int height, int hits, Texture2D image, QuadTree quad, SpriteBatch spriteBatch)
            : base(x, y, width, height, hits, image, quad, spriteBatch)
        {
            beingHeld = false;
            under = false;
        }
        #endregion Constructor

        #region Properties
        public bool BeingHeld
        {
            get { return beingHeld; }

            set { beingHeld = value; }
        }

        public bool Under
        {
            get { return under; }

            set { under = value; }
        }
        #endregion Properties

        #region Methods
        public void CheckStuff(Player plyr, KeyboardState kbState, GamePadState gamePad, Game1 gme)
        {
            // If a barricade collided with a player this code is called, and if the player is pressing 'Q' when it happened, stuff happens.
            // Or, in the case of a controller, X.
            if (plyr.HoldingBarricade || beingHeld)
            {
                //if player is holding the barricade and a weapon is selected, switch to it
                //repeat for all key combos
                if (gamePad.IsConnected)
                {
                    if (gamePad.IsButtonUp(Constants.BARRICADE_BUTTON))
                    {
                        plyr.HoldingBarricade = false;
                        beingHeld = false;
                        plyr.UnderBarricade = true;
                        plyr.CurrentItem = 0;
                    }
                }
                else
                {
                    int weaponNum = Constants.WEAPON_CHOSEN(kbState); // Returns what weapon you selected or -1 if no weapon selected
                    if (gme.SingleKeyPress(Constants.BARRICADE_KEY) || weaponNum >= 0)
                    {
                        plyr.HoldingBarricade = false;
                        beingHeld = false;
                        plyr.UnderBarricade = true;

                        plyr.CurrentItem = 0;
                        if (weaponNum >= 0)
                            plyr.CurrentItem = weaponNum;
                    }
                }
            }

            //has a plyr.UnderBarricade check so that holding the Q button won't pick it up after dropping it
            else if (!plyr.HoldingBarricade && !beingHeld && !plyr.UnderBarricade)
            {
                if (gamePad.IsConnected)
                {
                    if (gamePad.IsButtonDown(Constants.BARRICADE_BUTTON))
                    {
                        plyr.HoldingBarricade = true;
                        beingHeld = true;
                        plyr.UnderBarricade = true;
                        plyr.CurrentItem = 0;
                    }
                }
                else
                {
                    if (gme.SingleKeyPress(Constants.BARRICADE_KEY))
                    {
                        plyr.HoldingBarricade = true;
                        beingHeld = true;
                        plyr.UnderBarricade = true;
                        plyr.CurrentItem = 0;
                    }
                }
            }
        }

        public void Move(Player player, double dir)
        {
            beingHeld = true;
            under = true;

            tempX = player.rectangleX;
            tempY = player.rectangleY;

            rectangleX = (int)tempX;
            rectangleY = (int)tempY;

            Center = player.Center;
        }
        #endregion Methods

        #region Draws the image to the screen
        public override void Draw()
        {
            if (IsActive)
                base.Draw();
        }
        #endregion Draws the image to the screen
    }
}
