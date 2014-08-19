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
/// Controls the basic stats for the player

namespace TWEWZ
{
    public class Player : GameObject
    {
        #region Attributes
        private int health, prevX, prevY, speed, slowSpeed, currentItem, previousItem, xOffset, yOffset;
        private bool alive, hit, canMoveBarricade, underBarricade, holdingBarricade, barrCollisions, canAttack, isAttacking;
        private Rectangle outerBounds, rtnglSrc;
        private Color clr;
        #endregion Attributes

        #region Constructor
        public Player(int x, int y, int width, int height, Texture2D image, QuadTree quad, int difficulty, SpriteBatch spriteBatch)
            : base(image, x, y, width, height, spriteBatch)
        {
            health = Constants.PLAYER_HEALTH;
            Quad = quad;
            prevX = x; prevY = y;
            alive = true; canAttack = true;
            speed = 2;
            slowSpeed = speed / 2;
            canMoveBarricade = false; underBarricade = false; holdingBarricade = false;
            previousItem = currentItem = 0;
            outerBounds = new Rectangle(x - Constants.PLAYER_OUTER_BOUNDS * ((2 + difficulty) / 2), y - Constants.PLAYER_OUTER_BOUNDS * ((2 + difficulty) / 2), width + (Constants.PLAYER_OUTER_BOUNDS * 2 * ((2 + difficulty) / 2)), height + (Constants.PLAYER_OUTER_BOUNDS * 2 * ((2 + difficulty) / 2)));
        }
        #endregion Constructor

        #region Properties
        // Keeps track of the X cooridinate from last frame
        public int PrevX
        {
            get { return prevX; }

            set { prevX = value; }
        }

        // Keeps track of the Y cooridinate from last frame
        public int PrevY
        {
            get { return prevY; }

            set { prevY = value; }
        }

        public int Speed
        {
            get { return speed; }

            set { speed = value; }
        }

        public int SlowSpeed
        {
            get { return slowSpeed; }

            set { slowSpeed = value; }
        }

        public int Health
        {
            get { return health; }

            set { health = value;
            if (health > Constants.PLAYER_HEALTH)
                health = Constants.PLAYER_HEALTH;
            }
        }

        public bool Hit
        {
            get { return hit; }
            set { hit = value; }
        }

        public bool Alive
        {
            get { return alive; }
            set { alive = value; }
        }

        public Rectangle OuterBounds
        {
            get { return outerBounds; }

            set { outerBounds.X = value.X - Constants.PLAYER_OUTER_BOUNDS; outerBounds.Y = value.Y - Constants.PLAYER_OUTER_BOUNDS; }
        }

        public bool CanMoveBarricade
        {
            get { return canMoveBarricade; }

            set { canMoveBarricade = value; }
        }

        public bool UnderBarricade
        {
            get { return underBarricade; }

            set { underBarricade = value; }
        }

        public bool HoldingBarricade
        {
            get { return holdingBarricade; }

            set { holdingBarricade = value; }
        }

        public int CurrentItem
        {
            get { return currentItem; }

            set { if (currentItem != value) { previousItem = currentItem; } currentItem = value; }
        }

        public int PreviousItem
        {
            get { return previousItem; }

            set { previousItem = value; }
        }

        public bool CanAttack
        {
            get { return canAttack; }

            set { canAttack = value; }
        }

        public bool IsAttacking
        {
            get { return isAttacking; }

            set { isAttacking = value; }
        }
        #endregion Properties

        #region Methods
        public void TakeDamage(int damage)
        {
            this.health -= damage;
            this.hit = true;
            if (health <= 0)
                alive = false;
        }

        //work on these
        public bool CheckCollision(GameObject GO)
        {
            if (IsActive)
            {
                if (Intersects(GO.Rectangle))
                    return true;
                return false;
            }
            else
                return false;
        }

        public void GoBack(int goBackBy)
        {
            rectangleX += (int)(Math.Cos(Direction) * (goBackBy));
            rectangleY += (int)(Math.Sin(Direction) * (goBackBy));
        }

        public void BarricadeStuff(Dictionary<string,int> stats, List<Barricade> barricades, int framesElapsed)
        {
            // Speed is slowed when carrying a barricade
            if(HoldingBarricade){
                Speed = slowSpeed;
                if (framesElapsed % 5 == 0)
                    stats["stamina"]--;
            }

            // Player can't move barricade if no stamina
            if (stats["stamina"] <= 0){
                if (currentItem == 11){
                    holdingBarricade = false;
                    foreach (Barricade bar in barricades) { bar.BeingHeld = false; }
                    underBarricade = true;
                    currentItem = 0;
                }
                stats["stamina"] = 0;
            }

            // Correctly changes the Player's underBarricade attribute.
            CheckIfUnderBarricade(barricades);
        }

        public void CheckIfUnderBarricade(List<Barricade> barricades)
        {
            // This code doesn't work as smoothly in collision code, and having it here makes it less clutered and easier to understand
            barrCollisions = false;
            if (underBarricade){ 
                // If not under, we don't care
                foreach (Barricade bar in barricades){
                    if (new Rectangle(prevX, prevY, Width, Height).Intersects(((Barricade)bar).Rectangle)){
                        if (Intersects((Barricade)bar))
                            barrCollisions = true;
                    }
                }
                // If player is colliding with a barricade, all is good in the world. Else if no longer colliding, let everything know.
                if (!barrCollisions){
                    underBarricade = false;
                    foreach (Barricade bar in barricades) { bar.Under = false; }
                }
            }
        }
        #endregion Methods

        #region Draws the image to the screen
        public void Draw(SpriteBatch sprite, float angle, Texture2D images, int lasagna, GameTime gameTime, int numFrames, SoundEffect fistPunch, SoundEffect pipeHit, bool soundEffectsOn)
        {
            Texture = images;
            SetImage(lasagna, ((int)(gameTime.TotalGameTime.TotalMilliseconds / Constants.FRAME_TIME)) % numFrames, numFrames, fistPunch, pipeHit, soundEffectsOn);

            clr = Color.White;
            if (Hit) { clr = Color.Red; Hit = false; }
            rtnglSrc = new Rectangle(xOffset, yOffset, 84, 42);

            if (IsAttacking){
                rtnglSrc = new Rectangle(xOffset + rtnglSrc.Width * (((int)(gameTime.TotalGameTime.TotalMilliseconds / Constants.FRAME_TIME)) % numFrames), yOffset, (int)rtnglSrc.Width, (int)rtnglSrc.Height);
            }

            // works with 1/2 the texture height for both
            origin = new Vector2(42/2, 42/2);

            sprite.Draw(Texture, new Rectangle((int)Center.X, (int)Center.Y, Width * 2, Height), rtnglSrc, clr, angle, origin, SpriteEffects.None, 1f);
        }

        public void SetImage(int lasagna, int frame, int numFrames, SoundEffect fistPunch, SoundEffect pipeHit, bool soundEffectsOn)
        {
            //possibly can make more efficient by using a switch in draw
            if (currentItem == 0 || currentItem == 2 || currentItem == 4){
                if (currentItem == 0){
                    xOffset = 0;
                    yOffset = 0;
                }
                else if (currentItem == 2){
                    xOffset = 0;
                    yOffset = 42;
                }
                // currentItem == 4 is set lower down
                if (frame == 0)
                    canAttack = false;
                if (frame <= numFrames)
                    canAttack = true;
                if (frame == 0 && isAttacking){
                    if (soundEffectsOn){
                        if (currentItem == 0)
                            fistPunch.Play();
                        else if (currentItem == 2)
                            pipeHit.Play();
                    }
                }
            }
            if(currentItem == 11){
                xOffset = 0;
                yOffset = 0;
            }
            if(currentItem == 1){
                xOffset = 0;
                yOffset = 84;
            }
            if(currentItem >= 3 && currentItem <= 6){
                xOffset = (currentItem - 2) * 84;
                yOffset = 84;
            }
            if (currentItem >= 7 && currentItem <= 10){
                xOffset = (currentItem - 7) * 84;
                yOffset = 126;
            }

            if (lasagna <= 0 && currentItem == 10){
                xOffset = 0;
                yOffset = 0;
            }
        }
        #endregion Draws the image to the screen
    }
}
