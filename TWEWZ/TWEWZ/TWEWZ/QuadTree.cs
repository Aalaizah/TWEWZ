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
/// Quad tree class

namespace TWEWZ
{
    public class QuadTree
    {
        #region Attributes
        private List<GameObject> _objects, list, lst;
        private List<Rectangle> listRect, lstRect;
        private Rectangle _rect;
        private QuadTree _parent, tree;
        public QuadTree[] _divisions;
        #endregion Attributes

        #region Properties
        public QuadTree[] Divisions { get { return _divisions; } }

        public Rectangle Rectangle { get { return _rect; } }

        public List<GameObject> GameObjects { get { return _objects; } }

        public QuadTree Parent { get { return _parent; } }
        #endregion

        #region Constructor
        public QuadTree(int x, int y, int width, int height, QuadTree parent)
        {
            // Save the rectangle
            _rect = new Rectangle(x, y, width, height);

            // Create the object list
            _objects = new List<GameObject>();

            // No divisions yet
            _divisions = null;
            _parent = parent;
        }
        #endregion

        #region Methods
        public void AddObject(GameObject gameObj)
        {
            if (_rect.Contains(gameObj.Rectangle))
            {
                if (_divisions == null)
                {
                    _objects.Add(gameObj);
                    gameObj.Quad = this;
                }
                else
                {
                    if (_divisions[0].Rectangle.Contains(gameObj.Rectangle))
                        _divisions[0].AddObject(gameObj);
                    else if (_divisions[1].Rectangle.Contains(gameObj.Rectangle))
                        _divisions[1].AddObject(gameObj);
                    else if (_divisions[2].Rectangle.Contains(gameObj.Rectangle))
                        _divisions[2].AddObject(gameObj);
                    else if (_divisions[3].Rectangle.Contains(gameObj.Rectangle))
                        _divisions[3].AddObject(gameObj);
                    else if (_rect.Contains(gameObj.Rectangle))
                    {
                        _objects.Add(gameObj);
                        gameObj.Quad = this;
                    }
                }
            }
        }

        public void RemoveObject(GameObject gameObj)
        {
            tree = GetContainingQuad(gameObj.Rectangle);
            if(tree != null)
                tree.GameObjects.Remove(gameObj);
        }

        /// <summary>
        /// Divides this quad into 4 smaller quads.  Moves any game objects
        /// that are completely contained within the new smaller quads into
        /// those quads and removes them from this one.
        /// </summary>
        public void Divide()
        {
            _divisions = new QuadTree[4];
            _divisions[0] = new QuadTree(_rect.X, _rect.Y, _rect.Width / 2, _rect.Height / 2, this);
            _divisions[1] = new QuadTree(_rect.X + _rect.Width / 2, _rect.Y, _rect.Width / 2, _rect.Height / 2, this);
            _divisions[2] = new QuadTree(_rect.X, _rect.Y + _rect.Height / 2, _rect.Width / 2, _rect.Height / 2, this);
            _divisions[3] = new QuadTree(_rect.X + _rect.Width / 2, _rect.Y + _rect.Height / 2, _rect.Width / 2, _rect.Height / 2, this);
        }

        /// <summary>
        /// Recursively populates a list with all of the GameObjects in this
        /// quad and any subdivision quads
        /// </summary>
        /// <param name="rects">The reference list to populate</param>
        public List<GameObject> GetAllObjects()
        {
           list = new List<GameObject>();
            if (_objects.Count != 0)
            {
                foreach (GameObject obj in _objects)
                    list.Add(obj);
            }
            if (_divisions != null)
            {
                foreach (QuadTree tree in _divisions)
                {
                    lst = tree.GetAllObjects();
                    foreach(GameObject obj in lst)
                        list.Add(obj);
                }
            }
            return list;
        }

        /// <summary>
        /// Recursively populates a list with all of the rectangles in this
        /// quad and any subdivision quads
        /// </summary>
        public List<Rectangle> GetAllRectangles()
        {
            listRect = new List<Rectangle>();
            listRect.Add(_rect);
            if (_divisions != null)
            {
                foreach (QuadTree tree in _divisions)
                {
                    lstRect = tree.GetAllRectangles();
                    foreach (Rectangle rec in lstRect)
                        listRect.Add(rec);
                }
            }
            return listRect;
        }

        /// <summary>
        /// A possibly recursive method that returns the
        /// smallest quad that contains the specified rectangle
        /// </summary>
        public QuadTree GetContainingQuad(Rectangle rect)
        {
            tree = null;
            if (_rect.Contains(rect))
            {
                if (_divisions != null)
                {
                    if (_divisions[0].Rectangle.Contains(rect))
                        tree = _divisions[0].GetContainingQuad(rect);
                    else if (_divisions[1].Rectangle.Contains(rect))
                        tree = _divisions[1].GetContainingQuad(rect);
                    else if (_divisions[2].Rectangle.Contains(rect))
                        tree = _divisions[2].GetContainingQuad(rect);
                    else if (_divisions[3].Rectangle.Contains(rect))
                        tree = _divisions[3].GetContainingQuad(rect);
                    // If no sub-divisions contain, then it's contained in this rectangle
                    else
                        tree = this;
                }
                // If no subdivisions, nothing more to check.
                else
                    tree = this;
            }
            return tree;
        }

        /// <summary> Calls GetContainingQuad(Rectangle) and passes it GameObject.Position </summary>
        public QuadTree GetContainingQuad(GameObject GO)
        {
            return GetContainingQuad(GO.Rectangle);
        }
        #endregion
    }
}
