using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace Eve
{
    class HIV : Enemy
    {
       // public TimeSpan ReloadTime
        public TimeSpan MAX_INACTIVE_TIME = TimeSpan.FromSeconds(10.0);
        public TimeSpan curTime;
        public bool dormant = false;

        public HIV(Level level, Vector2 position) : base(level, position)
        {
            MoveSpeed = 0;
            MaxWaitTime = 0.1f;
            LoadContent("HIV");
            killIndex = 2;
        }

        /// <summary>
        /// Loads a particular enemy sprite sheet and sounds.
        /// </summary>
        public new void LoadContent(string spriteSet)
        {
            // Load animations.
            spriteSet = "Sprites/" + spriteSet + "/";
            runAnimation = new Animation(Level.Content.Load<Texture2D>(spriteSet + "Idle"), 0.1f, true);
            idleAnimation = new Animation(Level.Content.Load<Texture2D>(spriteSet + "Idle"), 0.15f, true);
            grayAnimation = new Animation(Level.Content.Load<Texture2D>(spriteSet + "Gray"), 0.15f, false);

            sprite.PlayAnimation(idleAnimation);

            dieSound = Level.Content.Load<SoundEffect>("Sounds/HIVDeath");

            // Calculate bounds within texture size.
            int width = (int)(idleAnimation.FrameWidth * 0.85);
            int left = (idleAnimation.FrameWidth - width) / 2;
            int height = (int)(idleAnimation.FrameWidth * 0.72);
            int top = idleAnimation.FrameHeight - height;
            localBounds = new Rectangle(left, top, width, height);
        }

        /// <summary>
        /// Paces back and forth along a platform, waiting at either end.
        /// </summary>
        public override void Update(GameTime gameTime)
        {
            if (dormant)
            {
                curTime -= gameTime.ElapsedGameTime;

                if (curTime < TimeSpan.Zero)
                    dormant = false;

                return;
            }

            if (position.X > Level.Player.Position.X)
                direction = (FaceDirection)(-1);
            else direction = (FaceDirection)(1);
        }

        public override void OnKilled()
        {
            dormant = true;
            curTime = MAX_INACTIVE_TIME; //set the timer to the sentinel

            dieSound.Play();
        }

        /// <summary>
        /// Draws the animated enemy.
        /// </summary>
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch, Color color, Vector2 screen, bool freeze = false)
        {
            if (dormant)
                sprite.PlayAnimation(grayAnimation);
            else
                sprite.PlayAnimation(idleAnimation);
            
            // Draw facing the way the enemy is moving.
            SpriteEffects flip = direction > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            sprite.Draw(gameTime, spriteBatch, Position - screen, color, flip, freeze);
        }
    }
}
