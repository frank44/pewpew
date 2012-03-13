using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;

namespace Platformer
{
    class Shot
    {
        private Texture2D texture;
        private Vector2 origin;
        private SoundEffect shotSound;

        public readonly Color Color = Color.Yellow;

        private Vector2 position;
        private Vector2 basePosition;
        private float velocity = 500.0f;
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

        private int direction;
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

        public Shot(Level level, Vector2 position, SpriteEffects flip, int inx, double y, double x)
        {
            this.shotIndex = inx;
            this.level = level;
            basePosition = position;
            this.position = position;

            y *= -1;

            if (flip == SpriteEffects.None) //shooting forward
            {
                direction = 1;
                if (x < 0)
                {
                    x *= -1;
                    y *= -1;
                }
            }
            else 
            {
                direction = -1; //shooting back
                if (x >= 0)
                {
                    x *= -1;
                    y *= -1;
                }
            }

            time = 0.0f;

            angle = Math.Atan2(y, x);

            LoadContent();
        }

        public void LoadContent()
        {
            texture = Level.Content.Load<Texture2D>(String.Format("Sprites/vaccine{0}", shotIndex));
            
            shotAnimation = new Animation(texture, 0.1f, false);
            sprite.PlayAnimation(shotAnimation);

            origin = new Vector2(texture.Width / 2.0f, texture.Height / 2.0f);
            shotSound = Level.Content.Load<SoundEffect>("Sounds/SlingshotFire");

            // Calculate bounds within texture size.            2
            int width = (int)(shotAnimation.FrameWidth * 0.5);
            int left = (shotAnimation.FrameWidth - width) / 2;
            int height = (int)(shotAnimation.FrameWidth * 0.25);
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
            // Draw facing the way the enemy is moving.
            //SpriteEffects flip = direction > 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            sprite.Draw(gameTime, spriteBatch, position - screen, color, SpriteEffects.None, freeze);
        }


    }
}
