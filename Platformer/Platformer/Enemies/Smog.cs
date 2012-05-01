using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace Eve
{
    class Smog : Enemy
    {
        TimeSpan ReproductionTime = TimeSpan.FromSeconds(5.0);
        TimeSpan curTime;
        Level lev;

        public Smog(Level level, Vector2 position)
            : base(level, position)
        {
            MoveSpeed = 120;
            MaxWaitTime = 0.5f;
            curTime = ReproductionTime;
            lev = level;

            LoadContent("Smog");
            killIndex = 0;
        }

        /// <summary>
        /// Loads a particular enemy sprite sheet and sounds.
        /// </summary>
        public new void LoadContent(string spriteSet)
        {
            // Load animations.
            spriteSet = "Sprites/" + spriteSet + "/";
            runAnimation = new Animation(Level.Content.Load<Texture2D>(spriteSet + "Run"), 0.1f, true);
            idleAnimation = new Animation(Level.Content.Load<Texture2D>(spriteSet + "Idle"), 0.15f, true);
            deathAnimation = new Animation(Level.Content.Load<Texture2D>(spriteSet + "Die"), 0.15f, true);
            sprite.PlayAnimation(idleAnimation);

            dieSound = Level.Content.Load<SoundEffect>("Sounds/MalariaDeath"); //same as malaria dying sound

            // Calculate bounds within texture size.
            int width = (int)(idleAnimation.FrameWidth * 0.7);
            int left = (idleAnimation.FrameWidth - width) / 2;
            int height = (int)(idleAnimation.FrameWidth * 0.7);
            int top = (idleAnimation.FrameHeight - height) / 2;
            localBounds = new Rectangle(left, top, width, height);
        }

        /// <summary>
        /// Paces back and forth along a platform, waiting at either end.
        /// </summary>
        public override void Update(GameTime gameTime)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            float diffX = Level.Player.Position.X - position.X;
            float diffY = Level.Player.Position.Y - position.Y;
            diffX = Math.Abs(diffX);
            diffY = Math.Abs(diffY);

            if (diffX < Level.window.Width / 2 && diffY < Level.window.Height)
                curTime = curTime.Subtract(TimeSpan.FromSeconds(1.0 * elapsed));

            if (curTime.CompareTo(TimeSpan.Zero) <= 0)
            {

                Vector2 newPosition = Position - Level.camera.Position - sprite.Origin;

                // Do not draw if out of scope of the window.
                if (newPosition.X + idleAnimation.FrameWidth >= 0 && newPosition.X <= Level.window.Width
                    && newPosition.Y + idleAnimation.FrameHeight >= 0 && newPosition.Y <= Level.window.Height)
                {
                    Random r = new Random();

                    if (r.NextDouble() < .99)
                    {
                        Smog child = new Smog(lev,
                        new Vector2(position.X + (float)(30 * r.NextDouble() - 15),
                            position.Y + (float)(30 * r.NextDouble() - 15)));

                        while (child.handleObjectCollisions())
                        {
                            child = new Smog(lev,
                        new Vector2(position.X + (float)(30 * r.NextDouble() - 15),
                            position.Y + (float)(30 * r.NextDouble() - 15)));
                        }
                        
                        if (child.position.Y > Level.window.Height * .99)
                            child.position.Y = (Level.window.Height * 0.99f);

                        Level.Enemies.Add(child);
                    }
                }
                curTime = ReproductionTime;
            }

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
                if (handleObjectCollisions())
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

        public override void OnKilled()
        {
            alive = false;
            dieSound.Play();
            sprite.PlayAnimation(deathAnimation);
        }

        /// <summary>
        /// Draws the animated enemy.
        /// </summary>
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch, Color color, Vector2 screen, bool freeze = false)
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
            SpriteEffects flip = direction < 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            sprite.Draw(gameTime, spriteBatch, Position - screen, color, flip, freeze);
        }
        #region Clone


        /// <summary>
        /// Returns a copy of the current object.
        /// </summary>
        public override Enemy Clone()
        {
            Smog clone = new Smog(Level, Position);
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

            clone.curTime = curTime;
            return clone;
        }


        #endregion
    }
}
