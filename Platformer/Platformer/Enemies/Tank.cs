﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace Eve
{
    class Tank : Enemy
    {
        public TimeSpan MaxReloadTime = TimeSpan.FromSeconds(3.0);
        public TimeSpan curReloadTime = TimeSpan.FromSeconds(0.0);

        public Tank(Level level, Vector2 position)
            : base(level, position)
        {
            MoveSpeed = 0;
            MaxWaitTime = 0.1f;
            LoadContent("Tank");
            killIndex = 2;
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
            deathAnimation = new Animation(Level.Content.Load<Texture2D>(spriteSet + "Die"), 0.15f, false);

            sprite.PlayAnimation(idleAnimation);

            dieSound = Level.Content.Load<SoundEffect>("Sounds/HIVDeath");

            // Calculate bounds within texture size.
            int width = (int)(idleAnimation.FrameWidth * 0.80);
            int left = (idleAnimation.FrameWidth - width) / 2;
            int height = (int)(idleAnimation.FrameWidth * 0.55);
            int top = idleAnimation.FrameHeight - height;
            localBounds = new Rectangle(left, top, width, height);
        }

        /// <summary>
        /// Paces back and forth along a platform, waiting at either end.
        /// </summary>
        public override void Update(GameTime gameTime)
        {
            curReloadTime -= gameTime.ElapsedGameTime;
            if (curReloadTime < TimeSpan.Zero)
            {
                double val = Math.Cos(Math.PI / 4);

                Vector2 delta = Vector2.Zero;
                if (direction == FaceDirection.Left)
                    delta = new Vector2(-20, -58);
                else delta = new Vector2(20, -58);

                ShootSpread(delta, Math.PI / 50, Math.PI / 10, Math.PI / 6);

                curReloadTime = MaxReloadTime;
            }

            if (position.X > Level.Player.Position.X)
                direction = (FaceDirection)(-1);
            else direction = (FaceDirection)(1);
        }

        public void ShootSpread(Vector2 delta, double a, double b, double c)
        {
            HIVShot s1 = new HIVShot(Level, position + delta, Math.Sin(a), Math.Cos(a), direction);
            Level.EnemyShots.Add(s1);

            HIVShot s2 = new HIVShot(Level, position + delta, Math.Sin(b), Math.Cos(b), direction);
            Level.EnemyShots.Add(s2);

            HIVShot s3 = new HIVShot(Level, position + delta, Math.Sin(c), Math.Cos(c), direction);
            Level.EnemyShots.Add(s3);
        }

        public override void OnKilled()
        {
            alive = false;
            dieSound.Play();
        }

        /// <summary>
        /// Draws the animated enemy.
        /// </summary>
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch, Color color, Vector2 screen, bool freeze = false)
        {
            sprite.PlayAnimation(idleAnimation);

            // Draw facing the way the enemy is moving.
            SpriteEffects flip = direction > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            sprite.Draw(gameTime, spriteBatch, Position - screen, color, flip, freeze);
        }


        #region Clone


        /// <summary>
        /// Returns a copy of the current object.
        /// </summary>
        public override Enemy Clone()
        {
            Tank clone = new Tank(Level, Position);
            clone.dieSound = dieSound;
            clone.direction = direction;
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
