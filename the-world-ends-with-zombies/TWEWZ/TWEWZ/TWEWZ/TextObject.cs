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
/// Holds information need to display a text object

namespace TWEWZ
{
    class TextObject : GameObject
    {
        #region Attributes
        private string message;
        private SpriteFont font;
        #endregion Attributes

        #region Constructor
        public TextObject(string message, Vector2 loc, SpriteFont font, Color color, SpriteBatch spriteBatch)
            : base(null, (int)loc.X, (int)loc.Y, 1, 1, spriteBatch)
        {
            this.message = message;
            this.font = font;
            Color = color;
        }

        public TextObject(string message, int xx, int yy, SpriteFont font, Color color, SpriteBatch spriteBatch)
            : base(null, xx, yy, 1, 1, spriteBatch)
        {
            this.message = message;
            this.font = font;
            Color = color;
        }
        #endregion Constructor

        #region Properties
        public string Message
        {
            get { return message; }
            set { message = value; }
        }

        public SpriteFont Font
        {
            get { return font; }
            set { font = value; }
        }

        new public int Width
        {
            get { return (int)font.MeasureString(message).X; }
        }

        new public int Height
        {
            get { return (int)font.MeasureString(message).Y; }
        }

        new public Rectangle Rectangle
        {
            get { return new Rectangle(rectangleX, rectangleY, Width, Height); }
        }

        #endregion Properties

        #region Draw
        public override void Draw()
        {
            spriteBatch.DrawString(font, message, Point, color);
        }
        #endregion Draw
    }
}
