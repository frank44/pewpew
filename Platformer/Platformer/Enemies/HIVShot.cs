using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace Eve
{
    class HIVShot
    {
        private Texture2D texture;
        private Vector2 origin;
        private SoundEffect shotSound;

        private Vector2 position;
        private Vector2 basePosition;
        private float velocity = 300.0f;
        private float xvelocity;
        private float time;
        private float gravity = -2.0f;
        private double angle;

        public Rectangle localBounds;

        public Rectangle BoundingRectangle
        {
            get
            {
                int left = (int)Math.Round(Position.X - sprite.Origin.X) + localBounds.X;
                int top = (int)Math.Round(Position.Y - sprite.Origin.Y) + localBounds.Y;

                return new Rectangle(left, top, localBounds.Width, localBounds.Height);
            }

        }

        public AnimationPlayer sprite;
        public Animation shotAnimation;

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

        public HIVShot(Level level, Vector2 position, double y, double x, FaceDirection fd)
        {
            this.level = level;
            basePosition = position;
            this.position = position;

            if (fd == FaceDirection.Left)
            {
                xvelocity = -velocity;
                y *= -1;
            }
            else
            {
                xvelocity = velocity;
                y *= -1;
            }

            time = 0.0f;
            angle = Math.Atan2(y, x);
            LoadContent();
        }

        public void LoadContent()
        {
            texture = Level.Content.Load<Texture2D>("Sprites/HIV/HIVShot");
            
            shotAnimation = new Animation(texture, 0.1f, false);
            sprite.PlayAnimation(shotAnimation);

            origin = new Vector2(texture.Width / 2.0f, texture.Height / 2.0f);
            shotSound = Level.Content.Load<SoundEffect>("Sounds/SlingshotFire");
            //should maybe change the sound...

            // Calculate bounds within texture size.
            int width = (int)(shotAnimation.FrameWidth);
            int left = 0;
            int height = (int)(shotAnimation.FrameWidth);
            int top = 0;
            localBounds = new Rectangle(left, top, width, height);
        }

        public void Update(GameTime gameTime)
        {
            float t = (float)gameTime.ElapsedGameTime.TotalSeconds;
            time += t;
            Vector2 v = new Vector2((float)(xvelocity * t * Math.Cos(angle)), (float)(velocity * t * Math.Sin(angle) -gravity*time*time));
            position = position + v;
        }

        public void OnShot()
        {
            shotSound.Play();
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, Color color, Vector2 screen, bool freeze = false)
        {
            // Draw facing the way the enemy is moving.
            //SpriteEffects flip = direction > 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            sprite.Draw(gameTime, spriteBatch, position - screen, color, SpriteEffects.None, freeze);
        }
    }
}
