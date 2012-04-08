#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using System.Collections.Generic;
#endregion

namespace Eve
{
    /// <summary>
    /// Class to represent an object in the game.
    /// </summary>
    class Object
    {
        #region Properties


        /// <summary>
        /// The type of the object.
        /// </summary>
        private string objectType;


        /// <summary>
        /// Position in world space of the bottom center of object.
        /// </summary>
        private Vector2 position;


        /// <summary>
        /// Position in world space of the bottom center of object.
        /// </summary>
        public Vector2 Position
        {
            get { return position; }
        }


        /// <summary>
        /// The rectangular parts that encompass an object. The first index represents
        /// the frame of the object's sprite and the second index represents a single part.
        /// </summary>
        private Part [][] parts;


        /// <summary>
        /// The rectangular parts that ecompass an object at its current frame in world space.
        /// </summary>
        public List<Part> Parts
        {
            get
            {
                List<Part> updatedParts = new List<Part>();
                foreach (Part part in parts[sprite.FrameIndex])
                {
                    int left = (int)Math.Round(Position.X - sprite.Origin.X) + part.BoundingRectangle.X;
                    int top = (int)Math.Round(Position.Y - sprite.Origin.Y) + part.BoundingRectangle.Y;
                    Part updatedPart = part.Clone();
                    updatedPart.BoundingRectangle = new Rectangle(left, top, part.BoundingRectangle.Width, part.BoundingRectangle.Height);
                    updatedParts.Add(updatedPart);
                }
                return updatedParts;
            }
        }


        /// <summary>
        /// The animation of a sprite.
        /// </summary>
        private Animation animation;


        /// <summary>
        /// The sprite of the object.
        /// </summary>
        private AnimationPlayer sprite;


        #endregion


        #region Initialization


        /// <summary>
        /// Constructs a new object
        /// </summary>
        public Object(string objectType, Vector2 position)
        {
            this.objectType = objectType;
            this.position = position;
            parts = ObjectManager.getParts(objectType);
            LoadContent();
        }


        /// <summary>
        /// Loads a particular object sprite sheet.
        /// </summary>
        public virtual void LoadContent()
        {
            // Load animations.
            string spriteLocation = @"Sprites\Objects\" + objectType;
            Texture2D spriteSheet = Session.ScreenManager.Game.Content.Load<Texture2D>(spriteLocation);
            animation = new Animation(spriteSheet, 0.1f, true, parts.GetLength(0));
            sprite.PlayAnimation(animation);
        }


        #endregion


        #region Update and Draw


        /// <summary>
        /// Update the sprite and the characteristics of the current object.
        /// </summary>
        public virtual void Update(GameTime gameTime)
        {
            //sprite.PlayAnimation(animation);
        }


        /// <summary>
        /// Draws the object
        /// </summary>
        public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch, Vector2 screen, Color color)
        {
            sprite.Draw(gameTime, spriteBatch, Position - screen, color, SpriteEffects.None, true);
        }


        #endregion
    }
}
