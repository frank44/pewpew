﻿using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;

namespace Eve
{
    class Shot
    {
        private Texture2D texture;
        private Vector2 origin;
        private SoundEffect shotSound;

        private Vector2 position;
        private Vector2 basePosition;
        private float velocity = 500.0f;
        private float time;
        private float gravity = -0.20f;
        private double angle;

        public Rectangle localBounds;

        public Rectangle BoundingRectangle
        {
            get
            {
                int left = (int)Math.Round(Position.X - sprite.Origin.X) + localBounds.X;
                int top = (int)Math.Round(Position.Y - sprite.Origin.Y/2) + localBounds.Y;

                int nh = (int)(localBounds.Width*Math.Sin(angle) + localBounds.Height*Math.Cos(angle));
                int nw = (int)(localBounds.Width*Math.Cos(angle) + localBounds.Height*Math.Sin(angle));
                return new Rectangle(left-nw+localBounds.Width, top-nh+localBounds.Height, nw, nh);
            }

        }

        private AnimationPlayer sprite;
        private Animation shotAnimation;
        public int shotIndex = 0;

        public Level Level
        {
            get { return level; }
        }
        Level level;

        /// <summary>
        /// Gets the current position of this gem in world space.
        /// </summary>
        public Vector2 Position
        {
            get
            {
                return position;
            }
        }

        public Shot(Level level, Vector2 position, int inx, double y, double x, SpriteEffects se)
        {
            this.shotIndex = inx;
            this.level = level;
            basePosition = position;
            this.position = position;

            if (x == 0 && y == 0 && se != SpriteEffects.None)
                velocity *= -1;

            time = 0.0f;
            angle = Math.Atan2(y, x);
            LoadContent();
        }

        public void LoadContent()
        {
            texture = Level.Content.Load<Texture2D>(String.Format("Sprites/vaccine/Level{0}/vaccine{1}", Session.StatisticsManager.LevelIndex, shotIndex));
            
            shotAnimation = new Animation(texture, 0.1f, false);
            sprite.PlayAnimation(shotAnimation);

            origin = new Vector2(texture.Width / 2.0f, texture.Height / 2.0f);
            shotSound = Level.Content.Load<SoundEffect>("Sounds/SlingshotFire");

            // Calculate bounds within texture size.
            int width = (int)(shotAnimation.FrameWidth *.5);
            width = (int)MathHelper.Clamp(width, 7, 32);

            int left = (shotAnimation.FrameWidth - width) / 2;
            int height = (int)(shotAnimation.FrameHeight *.16);
            height = (int)MathHelper.Clamp(height, 7, 32);

            int top = (shotAnimation.FrameHeight - height) / 2;
            localBounds = new Rectangle(left, top, width, height);
        }

        public void Update(GameTime gameTime)
        {
            float t = (float)gameTime.ElapsedGameTime.TotalSeconds;
            time += t;
            Vector2 v = new Vector2((float)(velocity * t * Math.Cos(angle)), (float)(velocity * t * Math.Sin(angle) -gravity*time*time));
            position = position + v;
        }

        public void OnShot()
        {
            shotSound.Play();
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, Color color, Vector2 screen, bool freeze = false)
        {
           //flip was removed since rotation took care of it
           // SpriteEffects flip = Math.Cos(angle)*velocity < 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            Vector2 v = new Vector2((float)(velocity * Math.Cos(angle)), (float)(velocity * Math.Sin(angle) + -gravity * time));
            float rot = (float)Math.Atan2(v.Y, v.X);

            sprite.Draw(gameTime, spriteBatch, position - screen, color, SpriteEffects.None, rot, freeze);
        }
    }
}

