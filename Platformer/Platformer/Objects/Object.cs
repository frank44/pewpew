#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
#endregion

namespace Platformer
{
    /// <summary>
    /// Class to represent an object in the game.
    /// </summary>
    abstract class Object
    {
        #region

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
        /// The bounding rectangle of all frames locally.
        /// </summary>
        protected Rectangle[] boundingRectangles;


        /// <summary>
        /// The bounding rectangle of the current frame locally.
        /// </summary>
        private Rectangle localBounds
        {
            get { return boundingRectangles[sprite.FrameIndex]; }
        }


        /// <summary>
        /// Gets a rectangle which bounds this enemy in world space.
        /// </summary>
        public Rectangle BoundingRectangle
        {
            get
            {
                int left = (int)Math.Round(Position.X - sprite.Origin.X) + localBounds.X;
                int top = (int)Math.Round(Position.Y - sprite.Origin.Y) + localBounds.Y;

                return new Rectangle(left, top, localBounds.Width, localBounds.Height);
            }
        }


        /// <summary>
        /// Determines whether an object can be passed through.
        /// </summary>
        private bool passable = false;


        /// <summary>
        /// Determines whether an object can be passed through.
        /// </summary>
        public bool Passable
        {
            get { return passable; }
            set { passable = value; }
        }


        /// <summary>
        /// Determines whether jumping on this object is bouncy.
        /// </summary>
        private bool bouncy = false;


        /// <summary>
        /// Determines whether jumping on this object is bouncy.
        /// </summary>
        public bool Bouncy
        {
            get { return bouncy; }
            set { bouncy = value; }
        }


        /// <summary>
        /// Determines whether an object can kill the player.
        /// </summary>
        private bool damaging = false;


        /// <summary>
        /// Determines whether an object can kill the player.
        /// </summary>
        public bool Damaging
        {
            get { return damaging; }
            set { damaging = value; }
        }


        private Animation animation;
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
            boundingRectangles = ObjectManager.getBounds(objectType);
        }

        /// <summary>
        /// Loads a particular object sprite sheet.
        /// </summary>
        public virtual void LoadContent()
        {
            // Load animations.
            string spriteSet = "Sprites/Object/" + objectType;
            animation = new Animation(Session.GameplayScreen.ScreenManager.Game.Content.Load<Texture2D>(spriteSet), 0.1f, true);
            sprite.PlayAnimation(animation);
        }


        #endregion


        #region Update and Draw


        /// <summary>
        /// Update the sprite and the characteristics of the current object.
        /// </summary>
        public virtual void Update(GameTime gameTime)
        {
            sprite.PlayAnimation(animation);
            string[] characteristics = ObjectManager.getCharacteristics(objectType, sprite.FrameIndex);
            passable = false;
            damaging = false;
            bouncy = false;
            foreach (string characteristic in characteristics)
            {
                if (characteristic == "passable")
                    passable = true;
                if (characteristic == "damaging")
                    damaging = true;
                if (characteristic == "bouncy")
                    bouncy = true;
            }
        }


        /// <summary>
        /// Draws the object
        /// </summary>
        public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch, Vector2 screen)
        {
            sprite.Draw(gameTime, spriteBatch, Position - screen, Color.White, SpriteEffects.None);
        }


        #endregion
    }
}
