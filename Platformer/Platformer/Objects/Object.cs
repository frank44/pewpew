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
    /// Enum describes the object.
    /// </summary>
    public enum ObjectClass
    {
        Standing,   // Standard object that is still in the environment
        Activate,   // Object that activates something when the player does something
        Trigger,    // Object that does something when triggered
        ProximityTrigger, // Object that triggers when the player is near it 
    }

    /// <summary>
    /// Class to represent an object in the game.
    /// </summary>
    class Object
    {
        #region Properties


        /// <summary>
        /// An ID that specifically characterizes an object.
        /// </summary>
        protected int objectID;


        /// <summary>
        /// An ID that specifically characterizes an object.
        /// </summary>
        public int ObjectID
        {
            get { return objectID; }
        }


        /// <summary>
        /// The type of the object.
        /// </summary>
        protected string objectType;


        /// <summary>
        /// Position in world space of the bottom center of object.
        /// </summary>
        protected Vector2 position;


        /// <summary>
        /// Position in world space of the bottom center of object.
        /// </summary>
        public Vector2 Position
        {
            get { return position; }
        }


        /// <summary>
        /// The current class of the object.
        /// </summary>
        public ObjectClass ObjectClass
        {
            get;
            protected set;
        }


        /// <summary>
        /// The rectangular parts that encompass an object. The first index represents
        /// the frame of the object's sprite and the second index represents a single part.
        /// </summary>
        protected Part[][] parts;


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
        protected Animation animation;


        /// <summary>
        /// The animation of a sprite.
        /// </summary>
        public Animation Animation
        {
            get { return animation; }
        }


        /// <summary>
        /// The sprite of the object.
        /// </summary>
        protected AnimationPlayer sprite;


        /// <summary>
        /// The sprite of the object.
        /// </summary>
        public AnimationPlayer Sprite
        {
            get { return sprite; }
        }


        #endregion


        #region Initialization


        /// <summary>
        /// Constructs a new object
        /// </summary>
        public Object(string objectType, Vector2 position, int objectID)
        {
            this.objectType = objectType;
            this.position = position;
            this.objectID = objectID;
            parts = ObjectManager.getParts(objectType);
            LoadContent();
            ObjectClass = ObjectClass.Standing;
        }


        /// <summary>
        /// Loads a particular object sprite sheet.
        /// </summary>
        public virtual void LoadContent(bool isLooping = false)
        {
            // Load animations.
            string spriteLocation = @"Sprites\Objects\" + objectType;
            Texture2D spriteSheet = Session.ScreenManager.Game.Content.Load<Texture2D>(spriteLocation);
            animation = new Animation(spriteSheet, 0.1f, isLooping, parts.GetLength(0));
            sprite.PlayAnimation(animation);
        }


        #endregion


        #region Update and Draw


        /// <summary>
        /// Update the sprite and the characteristics of the current object.
        /// </summary>
        public virtual void Update(GameTime gameTime) { }


        /// <summary>
        /// Draws the object
        /// </summary>
        public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch, Vector2 screen, Color color, bool freeze = true)
        {
            sprite.Draw(gameTime, spriteBatch, Position - screen, color, SpriteEffects.None, freeze);
        }


        #endregion


        #region Clone


        /// <summary>
        /// Returns a copy of the current object.
        /// </summary>
        public virtual Object Clone()
        {
            Object clone = new Object(objectType, Position, objectID);
            clone.sprite = sprite;
            return clone;
        }


        #endregion
    }
}
