#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.Diagnostics;
using System.IO;
using System.Threading;
#endregion using

/// The World Ends With Zombies
/// Man & Coffee Games
/// Creates, draws, and moves the enemies

namespace TWEWZ
{
    class Enemy : GameObject
    {
        #region Attributes
        private bool enraged, hit, withinRange, wthnRng, dropHealthOrItem, canDamage;
        private int i, health, prevX, prevY, damage, difficulty, wanderTime, type, color1, color2, color3, dropChance, frame;
        private double speed, exactX, exactY;
        private Random random;
        private Item drop;
        private Rectangle textureCutOut;
        private Color clr;
        #endregion Attributes

        #region Constructor
        public Enemy(int x, int y, int width, int height, int type, Texture2D image, QuadTree quad, int difficulty, Random rnd, bool randColors, SpriteBatch spriteBatch)
            : base(image, x, y, width, height, spriteBatch)
        {
            IsActive = true; enraged = false; withinRange = false;
            this.difficulty = difficulty;
            this.type = type;
            Quad = quad; prevX = x; prevY = y; exactX = x; exactY = y;

            //regular zombies
            if(type == 0){
                health = Constants.ZOMBIE_HEALTH;
                speed = 1 + difficulty;
                damage = Constants.ZOMBIE_DAMAGE + difficulty;
            }
            //flaming zombies
            else if (type == 1){
                health = Constants.ZOMBIE_HEALTH / 4;
                speed = 2.0 + difficulty;
                damage = Constants.ZOMBIE_DAMAGE + difficulty;
            }
            //zombie dogs
            else if (type == 2){
                health = Constants.ZOMBIE_HEALTH / 4;
                speed = 2.0 + difficulty;
                damage = Constants.ZOMBIE_DAMAGE + difficulty;
            }
            //doctor zombies
            else if (type == 3)
            {
                health = Constants.ZOMBIE_HEALTH * 2;
                speed = 0.25 + difficulty;
                damage = Constants.ZOMBIE_DAMAGE + difficulty;
            }
            //swat zombies
            else if (type == 4){
                health = Constants.ZOMBIE_HEALTH * 2;
                speed = 0.25 + difficulty;
                damage = Constants.ZOMBIE_DAMAGE + difficulty;
            }
            //boss zombie
            //else if (type == 5){
            //    health = Constants.ZOMBIE_HEALTH * 50;
            //    speed = 0.25 + difficulty;
            //    damage = 10 * Constants.ZOMBIE_DAMAGE + (difficulty * 2);
            //}

            random = rnd;
            Direction = random.Next(359);

            if (randColors)
                PickRandColor(0);
            else
                color = Color.White;

            wanderTime = (300 - random.Next(600)) + 1200;
        }
        #endregion Constructor

        #region Properties
        public double Speed
        {
            get { return speed; }
        }

        public bool Alive
        {
            get { return IsActive; }
        }

        public int Health
        {
            get { return health; }
        }

        //amount of damage a zombie can cause
        public int Damage
        {
            get { return damage; }
        }

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

        //returns whether or not a zombie is close
        public bool Enraged
        {
            get { return enraged; }
            set { enraged = value; }
        }

        //type of zombie
        public int Type
        {
            get { return type; }
            set { type = value; }
        }

        public bool WithinRange
        {
            get { return withinRange; }
            set { withinRange = value; }
        }
        #endregion Properties

        #region Move
        public void Move(GameObject player, int gameTime)
        {
            prevX = rectangleX; prevY = rectangleY;

            wthnRng = withinRange;
            EnemyPlayerRange((Player)player); //gets called to see if enemy is close enough to player to KILL
            
            // if the player is within the area, change direction, etc
            if (withinRange || enraged){
                Direction = (float)Math.Atan2(player.rectangleY - rectangleY, player.rectangleX - rectangleX);
                if (wthnRng != withinRange)
                    Wander(1200);
            }

            if (rectangleX <= Constants.PLAY_AREA_LEFT || rectangleX >= Constants.PLAY_AREA_RIGHT - Width || rectangleY <= Constants.PLAY_AREA_TOP || rectangleY >= Constants.PLAY_AREA_BOTTOM - Height)
                GoToGameArea();
            else
                Wander(gameTime);
            exactX += Math.Cos(Direction) * speed;
            exactY += Math.Sin(Direction) * speed;
            rectangleX = (int)exactX;
            rectangleY = (int)exactY;
        }

        public void Wander(int gameTime)
        {
            //randomize direction change
            if ((gameTime % 950 + random.Next(100, 750)) == 0){
                int directionChange = Constants.WANDER_DEGREE_CHANGE - random.Next(Constants.WANDER_DEGREE_CHANGE * 2);
                if (directionChange >= -Constants.MININUM_WANDER_DEGREE_CHANGE && directionChange < 0)
                    directionChange = -Constants.MININUM_WANDER_DEGREE_CHANGE;
                else if (directionChange <= Constants.MININUM_WANDER_DEGREE_CHANGE && directionChange >= 0)
                    directionChange = Constants.MININUM_WANDER_DEGREE_CHANGE;
                Direction += Constants.DegreeToRadian(directionChange);
            }
            GoToGameArea();
        }

        public void GoToGameArea()
        {
            if (!Constants.PLAY_AREA_Minus1.Contains(Rectangle)) {
                Direction = (float)Math.Atan2((float)(Constants.PLAY_AREA.Y + (Constants.PLAY_AREA.Height / 2)) - Center.Y, (float)(Constants.PLAY_AREA.X + (Constants.PLAY_AREA.Width / 2)) - Center.X);
                exactX += Math.Cos(Direction) * speed;
                exactY += Math.Sin(Direction) * speed;
                rectangleX = (int)exactX;
                rectangleY = (int)exactY;
            }
        }

        public void GoBack(int goBackBy)
        {
            Direction -= (float)Math.PI;
            exactX += Math.Cos(Direction) * (goBackBy);
            exactY += Math.Sin(Direction) * (goBackBy);
            rectangleX = (int)exactX;
            rectangleY = (int)exactY;
        }
        #endregion Move

        #region Other Methods
        public void TakeDamage(int damage, bool meleeHit)
        {
            if (health > 0){
                health -= damage;
                hit = true;
                enraged = true;
                speed += (damage * (2.7 / Constants.ZOMBIE_HEALTH));
            }
            else
                Killed();
        }

        public void Killed()
        {
            IsActive = false;
            PickRandColor(80);
            resize = 1.5f + (float)(random.NextDouble() * 0.25);
            //4 blood images
            //change to fit new sprite sheet placement of blood
            CutOut = new Rectangle(random.Next(4) * 42, 210, 42, 42);
        }

        public Item DropItem(Texture2D itemSpriteSheet, bool[] ownedWeapons, ref int wavesSinceDrop, int gameDifficulty)
        {
            drop = null;
            dropHealthOrItem = false;
            dropChance = 20 + (gameDifficulty * 7);

            if (random.Next(10) > random.Next(dropChance)){
                textureCutOut = Rectangle.Empty;
                if (wavesSinceDrop >= 2){
                    bool noNewWeapon = true;
                    for (i = 0; i < ownedWeapons.Length; i++){
                        if (!ownedWeapons[i]){
                            if (i <= 4)
                                textureCutOut = new Rectangle(i * 42, 0, 42, 42);
                            else
                                textureCutOut = new Rectangle((i - 5) * 42, 42, 42, 42);
                            drop = new Item(rectangleX, rectangleY, Constants.ITEM_SIZE_X, Constants.ITEM_SIZE_Y, i, itemSpriteSheet, textureCutOut, spriteBatch);
                            wavesSinceDrop = 0;
                            noNewWeapon = false;
                            break;
                        }
                    }
                    if (noNewWeapon){
                        wavesSinceDrop = -Int32.MaxValue;
                        dropHealthOrItem = true;
                    }
                } 
                else {
                    dropHealthOrItem = true;
                }

                if (dropHealthOrItem){
                    int rndNum = -random.Next(1, 3);
                    if (rndNum == -1)
                        textureCutOut = new Rectangle(0, 84, 42, 42);
                    else if (rndNum == -2)
                        textureCutOut = new Rectangle(42, 84, 42, 42);

                    if (rndNum != -3)
                        drop = new Item(rectangleX, rectangleY, Constants.ITEM_SIZE_X, Constants.ITEM_SIZE_Y, rndNum, itemSpriteSheet, textureCutOut, spriteBatch);
                }
            }
            return drop;
        }

        public void AttackPlayer(Player player)
        {
            if (canDamage){
                player.TakeDamage(damage);
            }
            GoBack(4);
        }

        public bool CheckCollision(GameObject GO)
        {
            if (IsActive){
                if (Intersects(GO.Rectangle))
                    return true;
                return false;
            }
            else
                return false;
        }

        private void EnemyPlayerRange(Player player)
        {
            // checks to see if player is in detection range
            if (Intersects(player.OuterBounds))
                withinRange = true;
            else
                withinRange = false;
        }

        public void PickRandColor(int minDarkness)
        {
            Constants.Wait(1);
            color1 = minDarkness + random.Next(255 - minDarkness);
            Constants.Wait(1);
            color2 = minDarkness + random.Next(255 - minDarkness);
            Constants.Wait(1);
            color3 = minDarkness + random.Next(255 - minDarkness);
            color = Color.FromNonPremultiplied(color1, color2, color3, 255);
        }
        #endregion Other Methods

        #region Draws the image to the screen
        public void Draw(Player player, GameTime gameTime/*, Boolean hats*/)
        { 
            if (!IsActive){
                //draws blood if a zombie is killed
                spriteBatch.Draw(Texture, new Rectangle(rectangleX + (int)(((Width * resize) - (Width / 2)) / 2), rectangleY + (int)(((Height * resize) - (Height / 2)) / 2), (int)(Width * resize), (int)(Height * resize)), CutOut, color, Direction, new Vector2(cutOutWidth / 2, cutOutHeight / 2), SpriteEffects.None, 0);
            }
            else {
                clr = color;
                if (hit) { clr = Color.Red; hit = false; }

                canDamage = false;

                //passive animation cycles
                if (type == 0){
                    frame = (int)((gameTime.TotalGameTime.TotalMilliseconds / Constants.ZOMBIE_FRAME_TIME) % 4);
                    if(frame == 0)
                        spriteBatch.Draw(Texture, new Rectangle((int)Center.X, (int)Center.Y, Width * 2, Height), new Rectangle(0, type * 42, 84, 42), clr, Direction, new Vector2(42 / 2, 42 / 2), SpriteEffects.None, 0);
                    else if(frame == 1)
                        spriteBatch.Draw(Texture, new Rectangle((int)Center.X, (int)Center.Y, Width * 2, Height), new Rectangle(84, type * 42, 84, 42), clr, Direction, new Vector2(42 / 2, 42 / 2), SpriteEffects.None, 0);
                    else if (frame == 2)
                        spriteBatch.Draw(Texture, new Rectangle((int)Center.X, (int)Center.Y, Width * 2, Height), new Rectangle(168, type * 42, 84, 42), clr, Direction, new Vector2(42 / 2, 42 / 2), SpriteEffects.None, 0);
                    else if (frame == 3){
                        spriteBatch.Draw(Texture, new Rectangle((int)Center.X, (int)Center.Y, Width * 2, Height), new Rectangle(252, type * 42, 84, 42), clr, Direction, new Vector2(42 / 2, 42 / 2), SpriteEffects.None, 0);
                        canDamage = true;
                    }
                }
                else if (type == 1){
                    frame = (int)((gameTime.TotalGameTime.TotalMilliseconds / Constants.ZOMBIE_FRAME_TIME) % 2);
                    if (frame == 0)
                        spriteBatch.Draw(Texture, new Rectangle((int)Center.X, (int)Center.Y, Width * 2, Height), new Rectangle(0, type * 42, 84, 42), clr, Direction, new Vector2(42 / 2, 42 / 2), SpriteEffects.None, 0);
                    else if (frame == 1){
                        spriteBatch.Draw(Texture, new Rectangle((int)Center.X, (int)Center.Y, Width * 2, Height), new Rectangle(84, type * 42, 84, 42), clr, Direction, new Vector2(42 / 2, 42 / 2), SpriteEffects.None, 0);
                        canDamage = true;
                    }
                }
                else if (type == 2){
                    frame = (int)((gameTime.TotalGameTime.TotalMilliseconds / Constants.ZOMBIE_FRAME_TIME) % 2);
                    spriteBatch.Draw(Texture, new Rectangle((int)Center.X, (int)Center.Y, Width * 2, Height), new Rectangle(0, type * 42, 84, 42), clr, Direction, new Vector2(42 / 2, 42 / 2), SpriteEffects.None, 0);
                    if(frame == 2)
                        canDamage = true;
                }
                else if (type == 3){
                    frame = (int)((gameTime.TotalGameTime.TotalMilliseconds / Constants.ZOMBIE_FRAME_TIME) % 5);
                    if (frame == 0)
                        spriteBatch.Draw(Texture, new Rectangle((int)Center.X, (int)Center.Y, Width * 2, Height), new Rectangle(0, type * 42, 84, 42), clr, Direction, new Vector2(42 / 2, 42 / 2), SpriteEffects.None, 0);
                    else if (frame == 1)
                        spriteBatch.Draw(Texture, new Rectangle((int)Center.X, (int)Center.Y, Width * 2, Height), new Rectangle(84, type * 42, 84, 42), clr, Direction, new Vector2(42 / 2, 42 / 2), SpriteEffects.None, 0);
                    else if (frame == 2)
                        spriteBatch.Draw(Texture, new Rectangle((int)Center.X, (int)Center.Y, Width * 2, Height), new Rectangle(168, type * 42, 84, 42), clr, Direction, new Vector2(42 / 2, 42 / 2), SpriteEffects.None, 0);
                    else if (frame == 3)
                        spriteBatch.Draw(Texture, new Rectangle((int)Center.X, (int)Center.Y, Width * 2, Height), new Rectangle(252, type * 42, 84, 42), clr, Direction, new Vector2(42 / 2, 42 / 2), SpriteEffects.None, 0);
                    else if (frame == 4){
                        spriteBatch.Draw(Texture, new Rectangle((int)Center.X, (int)Center.Y, Width * 2, Height), new Rectangle(336, type * 42, 84, 42), clr, Direction, new Vector2(42 / 2, 42 / 2), SpriteEffects.None, 0);
                        canDamage = true;
                    }
                }
                else if (type == 4)
                {
                    frame = (int)((gameTime.TotalGameTime.TotalMilliseconds / Constants.ZOMBIE_FRAME_TIME) % 4);
                    if (frame == 0)
                        spriteBatch.Draw(Texture, new Rectangle((int)Center.X, (int)Center.Y, Width * 2, Height), new Rectangle(0, type * 42, 84, 42), clr, Direction, new Vector2(42 / 2, 42 / 2), SpriteEffects.None, 0);
                    else if (frame == 1)
                        spriteBatch.Draw(Texture, new Rectangle((int)Center.X, (int)Center.Y, Width * 2, Height), new Rectangle(84, type * 42, 84, 42), clr, Direction, new Vector2(42 / 2, 42 / 2), SpriteEffects.None, 0);
                    else if (frame == 2)
                        spriteBatch.Draw(Texture, new Rectangle((int)Center.X, (int)Center.Y, Width * 2, Height), new Rectangle(168, type * 42, 84, 42), clr, Direction, new Vector2(42 / 2, 42 / 2), SpriteEffects.None, 0);
                    else if (frame == 3){
                        spriteBatch.Draw(Texture, new Rectangle((int)Center.X, (int)Center.Y, Width * 2, Height), new Rectangle(252, type * 42, 84, 42), clr, Direction, new Vector2(42 / 2, 42 / 2), SpriteEffects.None, 0);
                        canDamage = true;
                    }
                }
                /*if (hats)
                {
                    //draw hats
                    spriteBatch.Draw(Texture, new Rectangle((int)Center.X, (int)Center.Y, Width * 2, Height), new Rectangle(210, 42, 42, 42), clr, Direction, new Vector2(cutOutHeight / 2, cutOutHeight / 2), SpriteEffects.None, 0);
                }*/
            } 
        }
        #endregion Draws the image to the screen
    }
}
