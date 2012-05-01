using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace Eve
{
    class Sludge : Enemy
    {

        public Sludge(Level level, Vector2 position)
            : base(level, position)
        {
            MoveSpeed = 200;
            MaxWaitTime = 0.1f;
            LoadContent("Sludge");
            killIndex = 1;
        }

        /// <summary>
        /// Loads a particular enemy sprite sheet and sounds.
        /// </summary>
        public new void LoadContent(string spriteSet)
        {
            // Load animations.
            spriteSet = "Sprites/" + spriteSet + "/";
            runAnimation = idleAnimation = new Animation(Level.Content.Load<Texture2D>(spriteSet + "Run"), 0.15f, true);
            deathAnimation = new Animation(Level.Content.Load<Texture2D>(spriteSet + "Die"), 0.15f, false);

            //NEED TO HAVE THIS, or else origin won't be set
            sprite.PlayAnimation(idleAnimation);

            dieSound = Level.Content.Load<SoundEffect>("Sounds/TuberculosisDeath"); //same for now

            // Calculate bounds within texture size.
            int width = (int)(runAnimation.FrameWidth * .95);
            int left = (int)((runAnimation.FrameWidth - width)*0.55);
            int height = (int)(runAnimation.FrameWidth);
            int top = runAnimation.FrameHeight - height;
            localBounds = new Rectangle(left, top, width, height);
        }

        /// <summary>
        /// Paces back and forth along a platform, waiting at either end.
        /// </summary>
        public override void Update(GameTime gameTime)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Calculate tile position based on the side we are walking towards.

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
                    waitTime = MaxWaitTime; //you must have hit something, enter idle state
                }
                else //move
                {
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
            if (!Level.Player.IsAlive || Level.ReachedExit || waitTime > 0)
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
            Sludge clone = new Sludge(Level, Position);
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
            clone.deathAnimation = deathAnimation;
            return clone;
        }


        #endregion
    }
}
