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
#endregion using

/// The World Ends With Zombies
/// Man & Coffee Games
/// This class allows you to draw an image, including from a sprite sheet, either as an animation or
/// single image, rotate or scale it, and has methods to check collision between game objects and
/// drawing a rectangle outline.
namespace TWEWZ
{
    public class GameObject
    {
        #region Attributes
        protected Vector2 origin;
        protected SpriteEffects effects;
        protected SpriteBatch spriteBatch;
        protected Texture2D texture;
        protected Color color;
        protected int xLoc, yLoc, width, height, xOffSet, yOffSet, cutOutWidth, cutOutHeight, totalFrames, cB;
        protected float resize, rotation;

        private bool isActive;
        private QuadTree quad;

        public Rectangle resizedRect;
        public Rectangle position;
        #endregion Attributes

        #region Constructors
        /// <summary> Default Constructor. </summary>
        public GameObject()
        {
            texture = null; spriteBatch = null;
            width = 1; height = 1;
            xLoc = 0; yLoc = 0; xOffSet = 0; yOffSet = 0; cutOutWidth = 1; cutOutHeight = 1; totalFrames = 1;
            color = Color.White;
            origin = Vector2.Zero;
            effects = SpriteEffects.None;
            rotation = 0; resize = 1f;
        }

        /// <summary> Basic Drawable GameObject. </summary>
        public GameObject(Texture2D texture, SpriteBatch spriteBatch)
        {
            this.texture = texture;
            this.spriteBatch = spriteBatch;

            xLoc = 0; yLoc = 0;
            this.width = 10; this.height = 10;
            if(texture != null){
                xOffSet = 0; yOffSet = 0;
                cutOutWidth = texture.Width; cutOutHeight = texture.Height;
                if (cutOutWidth <= cutOutHeight)
                    origin = new Vector2(cutOutWidth);
                else
                    origin = new Vector2(cutOutHeight);
            }
            totalFrames = 1;
            color = Color.White;
            rotation = 0;
            effects = SpriteEffects.None;
            resize = 1f;
        }

        /// <summary> Standard Drawable GameObject. </summary>
        public GameObject(Texture2D texture, int x, int y, int width, int height, SpriteBatch spriteBatch)
        {
            this.texture = texture;
            this.spriteBatch = spriteBatch;
            xLoc = x; yLoc = y;
            this.width = width; this.height = height;

            if(texture != null){
                cutOutWidth = texture.Width; cutOutHeight = texture.Height;
                xOffSet = 0; yOffSet = 0;
                if (cutOutWidth <= cutOutHeight)
                    origin = new Vector2(cutOutWidth);
                else
                    origin = new Vector2(cutOutHeight);
            }
            totalFrames = 1;
            color = Color.White;
            rotation = 0;
            origin = Vector2.Zero;
            effects = SpriteEffects.None;
            resize = 1f;
        }

        /// <summary> Sets every value of a GameObject to something. </summary>
        public GameObject(Texture2D texture, int x, int y, int width, int height, int totalFrameNum, int xOffSet, int yOffSet, int cutOutWidth,
            int cutOutHeight, Color color, float rotation, Vector2 origin, SpriteEffects effects, SpriteBatch spriteBatch)
        {
            this.texture = texture;
            this.spriteBatch = spriteBatch;
            xLoc = x; yLoc = y;
            this.width = width; this.height = height;
            if(texture != null){
                this.xOffSet = xOffSet; this.yOffSet = yOffSet;
                this.cutOutWidth = cutOutWidth; this.cutOutHeight = cutOutHeight;
            }
            totalFrames = totalFrameNum;
            this.color = color;
            this.rotation = rotation;
            this.origin = origin;
            this.effects = effects;
            resize = 1f;
        }
        #endregion Constructors

        #region Properties

        #region Position Properties
        public Rectangle Rectangle
        {
            get { return new Rectangle(xLoc, yLoc, width, height); }

            set { xLoc = value.X; yLoc = value.Y; width = value.Width; height = value.Height; }
        }

        /// <summary> The X,Y Coordinate of the GameObject. </summary>
        public Vector2 Point
        {
            get { return new Vector2(xLoc, yLoc); }

            set { xLoc = (int)value.X; yLoc = (int)value.Y; }
        }

        #region RectangleX
        public int rectangleX
        {
            get { return xLoc; }

            set
            {
                if (!((this is Ammo) || (this is Enemy) || (this is Melee)))
                {
                    //since player is the largest object on screen, the player size is used to check the sides
                    if (value >= Constants.PLAY_AREA_LEFT && value <= Constants.PLAY_AREA_RIGHT - Width)
                    {
                        xLoc = value;
                    }
                    else if (value < Constants.PLAY_AREA_LEFT)
                    {
                        xLoc = Constants.PLAY_AREA_LEFT;
                    }
                    else if (value > Constants.PLAY_AREA_RIGHT - Width)
                    {
                        xLoc = Constants.PLAY_AREA_RIGHT - Width;
                    }
                }
                else
                {
                    xLoc = value;
                }
            }
        }
        #endregion RectangleX

        #region RectangleY
        public int rectangleY
        {
            get { return yLoc; }

            set
            {
                if (!(this is Ammo || this is Melee))
                {
                    if (value >= Constants.PLAY_AREA_TOP && value <= Constants.PLAY_AREA_BOTTOM - Height)
                    {
                        yLoc = value;
                    }
                    else if (value < Constants.PLAY_AREA_TOP)
                    {
                        yLoc = Constants.PLAY_AREA_TOP;
                    }
                    else if (value > Constants.PLAY_AREA_BOTTOM - Height)
                    {
                        yLoc = Constants.PLAY_AREA_BOTTOM - Height;
                    }
                }
                else
                {
                    yLoc = value;
                }
            }
        }
        #endregion RectangleY

        public int Width
        {
            get { return width; }

            set { width = value; }
        }

        public int Height
        {
            get { return height; }

            set { height = value; }
        }

        /// <summary> The Center of the rectangle of this GameObject. </summary>
        public Vector2 Center
        {
            get { return new Vector2(xLoc + Width / 2, yLoc + Height / 2); }

            set { xLoc = (int)value.X - (Width / 2); yLoc = (int)value.Y - (Height / 2); }
        }
        #endregion Position Properties

        #region Image Properties
        public Texture2D Texture
        {
            get { return texture; }

            set { texture = value; }
        }

        /// <summary> Used so that the Camera can draw things to the screen. </summary>
        public SpriteBatch SpriteBatch
        {
            get { return spriteBatch; }

            set { spriteBatch = value; }
        }

        public Color Color
        {
            get { return color; }

            set { color = value; }
        }

        /// <summary> The X location on the Texture to draw. </summary>
        public int XOffSet
        {
            get { return xOffSet; }

            set { xOffSet = value; }
        }

        /// <summary> The Y location on the Texture to draw. </summary>
        public int YOffSet
        {
            get { return yOffSet; }

            set { yOffSet = value; }
        }

        /// <summary> The Width from XOffSet on the Texture to draw. </summary>
        public int CutOutWidth
        {
            get { return cutOutWidth; }

            set { cutOutWidth = value; }
        }

        /// <summary> The Height from YOffSet on the Texture to draw. </summary>
        public int CutOutHeight
        {
            get { return cutOutHeight; }

            set { cutOutHeight = value; }
        }

        /// <summary> The area on the Texture to draw. </summary>
        public Rectangle CutOut
        {
            get { return new Rectangle(xOffSet, yOffSet, cutOutWidth, cutOutHeight); }

            set { if (value == null) { xOffSet = 0; yOffSet = 0; cutOutWidth = texture.Width; cutOutHeight = texture.Height; } else { xOffSet = value.X; yOffSet = value.Y; cutOutWidth = value.Width; cutOutHeight = value.Height; } }
        }

        /// <summary> Total Number of Frames in the animation (1 if no animation) - Animations go Left to Right on a SpriteSheet. </summary>
        public int TotalFrames
        {
            get { return totalFrames; }

            set { totalFrames = value; }
        }

        /// <summary> Zero / 2PI starts in the EAST direction </summary>
        public float Direction
        {
            get { return rotation; }

            set { rotation = value; }
        }

        public bool IsActive
        {
            get { return isActive; }

            set { isActive = value; }
        }

        public QuadTree Quad
        {
            get { return quad; }

            set { quad = value; }
        }

        public Vector2 Origin
        {
            get { return origin; }

            set { origin = value; }
        }

        public SpriteEffects Effects
        {
            get { return effects; }

            set { effects = value; }
        }

        public float Resize
        {
            get { return resize; }

            set { resize = value; }
        }
        #endregion Image Properties

        #endregion Properties

        #region Methods
        protected float DiaginalDist(float xx, float yy)
        {
            return (float)Math.Sqrt((double)Math.Pow(xx, 2) + (double)Math.Pow(yy, 2));
        }

        /// <summary> Checks collsion of this GameObject and a Rectangle </summary>
        public bool Intersects(Rectangle rect)
        {
            return Rectangle.Intersects(rect);
        }

        /// <summary> Checks collsion of this GameObject and another GameObject </summary>
        public bool Intersects(GameObject GO)
        {
            return Rectangle.Intersects(GO.Rectangle);
        }

        /// <summary> Returns a resized version of this GameObject. </summary>
        public Rectangle Resized()
        {
            resizedRect = Rectangle;
            if(resize != 1f && resize != 0f){
                resizedRect.X = xLoc - (int)(((width * resize) - width) / 2);
                resizedRect.Y = yLoc - (int)(((height * resize) - height) / 2);
                resizedRect.Width = (int)(width * resize);
                resizedRect.Height = (int)(height * resize);
            }
            return resizedRect;
        }
        #endregion Methods

        #region Draw
        /// <summary> Basic Draw Method of this GameObject. </summary>
        public virtual void Draw()
        {
            Rectangle position = Resized();
            if (texture != null)
                spriteBatch.Draw(texture, position, CutOut, color, rotation, origin, effects, 0);
        }

        /// <summary> Draws an animation (Left to Right on a SpriteSheet). </summary>
        public void Draw(GameTime gameTime, int frameSpeed)
        {
            position = Resized();
            if(texture != null){
                Rectangle cutOutAnimation = CutOut;
                cutOutAnimation.X = xOffSet + (cutOutWidth * ((int)(gameTime.TotalGameTime.TotalMilliseconds / frameSpeed) % totalFrames));
                spriteBatch.Draw(texture, position, cutOutAnimation, color, rotation, origin, effects, 0);
            }
        }
        #endregion Draw

        #region Draw Rectangle Outline
        /// <summary> Draws an outline around this GameObject. </summary>
        public void DrawRectangleOutline(int borderSize, Color color, Texture2D blankTex)
        {
            DrawRectangleOutline(Rectangle, borderSize, color, blankTex, spriteBatch);
        }

        /// <summary> Draws an outline around a rectangle. </summary>
        public static void DrawRectangleOutline(Rectangle rect, int borderSize, Color color, Texture2D blankTex, SpriteBatch sprite)
        {
            int cB = borderSize;
            if (sprite != null && blankTex != null) {
                // Draw the 4 lines based on borderSize - Top, right, bottom, left
                sprite.Draw(blankTex, new Rectangle(rect.X - cB, rect.Y - cB, rect.Width + (cB * 2), cB), color);
                sprite.Draw(blankTex, new Rectangle(rect.X - cB, rect.Y - cB, cB, rect.Height + (cB * 2)), color);
                sprite.Draw(blankTex, new Rectangle(rect.X + rect.Width, rect.Y - cB, cB, rect.Height + (cB * 2)), color);
                sprite.Draw(blankTex, new Rectangle(rect.X - cB, rect.Y + rect.Height, rect.Width + (cB * 2), cB), color);
            }
        }
        #endregion Draw Rectangle Outline
    }
}
