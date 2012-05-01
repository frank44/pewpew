#region File Description
//-----------------------------------------------------------------------------
// Enemy.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;

namespace Eve
{
    /// <summary>
    /// Facing direction along the X axis.
    /// </summary>
    enum FaceDirection
    {
        Left = -1,
        Right = 1,
    }

    /// <summary>
    /// A monster who is impeding the progress of our fearless adventurer.
    /// </summary>
    class Enemy
    {
        #region Properties

        //specifies what index bullet kills this enemy
        public int killIndex;

        public Level Level
        {
            get { return level; }
        }
        Level level;

        /// <summary>
        /// Position in world space of the bottom center of this enemy.
        /// </summary>
        public Vector2 Position
        {
            get { return position; }
        }
        public Vector2 position;

        public Rectangle localBounds;
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

        // Animations
        public Animation runAnimation;
        public Animation idleAnimation;
        public Animation grayAnimation;
        public Animation deathAnimation;
        public AnimationPlayer sprite;

        public SoundEffect dieSound;
        public bool alive;

        /// <summary>
        /// The direction this enemy is facing and moving along the X axis.
        /// </summary>
        public FaceDirection direction = FaceDirection.Left;

        /// <summary>
        /// How long this enemy has been waiting before turning around.
        /// </summary>
        public float waitTime;

        /// <summary>
        /// How long to wait before turning around.
        /// </summary>
        public float MaxWaitTime = 1.0f;

        /// <summary>
        /// The speed at which this enemy moves along the X axis.
        /// </summary>
        public float MoveSpeed = 120;


        #endregion


        #region Initialization


        /// <summary>
        /// Constructs a new Enemy.
        /// </summary>
        public Enemy() { } 
        public Enemy(Level level, Vector2 position)
        {
            this.alive = true;
            this.level = level;
            this.position = position;

            //LoadContent(spriteSet);
        }

        /// <summary>
        /// Loads a particular enemy sprite sheet and sounds.
        /// </summary>
        public void LoadContent(string spriteSet)
        {
            // Load animations.
            spriteSet = "Sprites/" + spriteSet + "/";
            runAnimation = new Animation(Level.Content.Load<Texture2D>(spriteSet + "Run"), 0.1f, true);
            idleAnimation = new Animation(Level.Content.Load<Texture2D>(spriteSet + "Idle"), 0.15f, true);
            sprite.PlayAnimation(idleAnimation);

            dieSound = Level.Content.Load<SoundEffect>("Sounds/MonsterKilled");

            // Calculate bounds within texture size.
            int width = (int)(idleAnimation.FrameWidth * 0.35);
            int left = (idleAnimation.FrameWidth - width) / 2;
            int height = (int)(idleAnimation.FrameWidth * 0.7);
            int top = idleAnimation.FrameHeight - height;
            localBounds = new Rectangle(left, top, width, height);
        }


        #endregion


        #region Update and Draw


        /// <summary>
        /// Paces back and forth along a platform, waiting at either end.
        /// </summary>
        public virtual void Update(GameTime gameTime)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            
            handleObjectCollisions();

            // Calculate tile position based on the side we are walking towards.
            float posX = Position.X + localBounds.Width / 2 * (int)direction;
            int tileX = (int)Math.Floor(posX / Tile.Width) - (int)direction;
            int tileY = (int)Math.Floor(Position.Y / Tile.Height);

            if (waitTime > 0)
            {
                // Wait for some amount of time.
                waitTime = Math.Max(0.0f, waitTime - (float)gameTime.ElapsedGameTime.TotalSeconds);
                if (waitTime <= 0.0f)
                {
                    // Then turn around.
                    direction = (FaceDirection)(-(int)direction);
                }
            }
            else
            {
                // If we are about to run into a wall or off a cliff, start waiting.
                if (Level.GetCollision(tileX + (int)direction, tileY - 1) == TileCollision.Impassable ||
                    Level.GetCollision(tileX + (int)direction, tileY) == TileCollision.Passable)
                {
                    waitTime = MaxWaitTime;
                }
                else
                {
                    // Move in the current direction.
                    Vector2 velocity = new Vector2((int)direction * MoveSpeed * elapsed, 0.0f);
                    position = position + velocity;
                }
            }
        }

        public virtual void OnKilled()
        {
            alive = false;
            dieSound.Play();
            //sprite.PlayAnimation(dieAnimation);
        }

        /// <summary>
        /// Draws the animated enemy.
        /// </summary>
        public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch, Color color, Vector2 screen, bool freeze = false)
        {
            // Stop running when the game is paused or before turning around.
            if (!Level.Player.IsAlive ||
                Level.ReachedExit ||
                waitTime > 0)
            {
                sprite.PlayAnimation(idleAnimation);
            }
            else
            {
                sprite.PlayAnimation(runAnimation);
            }

            // Draw facing the way the enemy is moving.
            SpriteEffects flip = direction > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            sprite.Draw(gameTime, spriteBatch, Position - screen, color, flip, freeze);
        }

        public bool handleObjectCollisions()
        {
            Rectangle bounds = BoundingRectangle;

            if (position.X < localBounds.Left)
            {
                position = new Vector2(localBounds.Left, Position.Y);
                return true;
            }

            if (position.X > level.Width * 40)
            {
                position.X = level.Width * 40;
                return true;
            }

            foreach (Object o in Level.Objects)
            {
                foreach (Part r in o.Parts)
                {
                    if (r.PartType == PartType.Solid)
                    {
                        Rectangle br = r.BoundingRectangle;
                        Vector2 intr = RectangleExtensions.GetIntersectionDepth(bounds, br);

                        if (intr != Vector2.Zero)
                        {
                            double depthX = intr.X;
                            //if (depthX > 0) depthX += 5;
                            //else depthX -= 5;

                            position = new Vector2(Position.X + (float)depthX, Position.Y);
                            return true;
                        }
                    }
                }
            }

            return false;
        }



        #endregion

        #region Clone


        /// <summary>
        /// Returns a copy of the current object.
        /// </summary>
        public virtual Enemy Clone()
        {
            Enemy clone = new Enemy(Level, Position);
            clone.dieSound = dieSound;
            clone.direction = direction;
            clone.grayAnimation = grayAnimation;
            clone.idleAnimation = idleAnimation;
            clone.killIndex = killIndex;
            clone.localBounds = localBounds;
            clone.MaxWaitTime = MaxWaitTime;
            clone.MoveSpeed = MoveSpeed;
            clone.runAnimation = runAnimation;
            clone.sprite = sprite;
            clone.waitTime = waitTime;
            return clone;
        }


        #endregion
    }
}
