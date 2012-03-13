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
    public abstract class Object
    {
        #region 


        public Level Level
        {
            get { return level; }
        }
        Level level;


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
        /// The bounding rectangle of a frame locally.
        /// </summary>
        private Rectangle localBounds;


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
        private bool passable;


        /// <summary>
        /// Determines whether an object can be passed through.
        /// </summary>
        public bool Passable
        {
            get { return passable; }
        }


        /// <summary>
        /// Determines whether an object can kill the player.
        /// </summary>
        private bool damaging;


        /// <summary>
        /// Determines whether an object can kill the player.
        /// </summary>
        public bool Damaging
        {
            get { return damaging; }
        }


        private Animation animation;
        private AnimationPlayer sprite;


        #endregion


        #region Initialization


        /// <summary>
        /// Constructs a new object
        /// </summary>
        public virtual Object(Level level, Vector2 position, string spriteSet)
        {
            this.level = level;
            this.position = position;

            LoadContent(spriteSet);
        }

        /// <summary>
        /// Loads a particular object sprite sheet.
        /// </summary>
        public virtual void LoadContent(string spriteSet)
        {
            // Load animations.
            spriteSet = "Sprites/Object/" + spriteSet;
            animation = new Animation(Level.Content.Load<Texture2D>(spriteSet), 0.1f, true);
            sprite.PlayAnimation(animation);
        }


        #endregion


        #region Update and Draw


        /// <summary>
        /// Don't really have anything implemented yet besides updating the animation.
        /// </summary>
        public void Update(GameTime gameTime)
        {
            sprite.PlayAnimation(animation);
        }


        /// <summary>
        /// Draws the object
        /// </summary>
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, Vector2 screen)
        {
            sprite.Draw(gameTime, spriteBatch, Position - screen, Color.White, SpriteEffects.None);
        }


        #endregion
    }
}
