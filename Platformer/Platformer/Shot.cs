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
        private float velocity = 2.0f;

        private int direction = 1; //right for now
        private AnimationPlayer sprite;

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

        public Circle BoundingCircle
        {
            get
            {
                return new Circle(Position, Tile.Width / 3.0f);
            }
        }

        public Shot(Level level, Vector2 position)
        {
            this.level = level;
            basePosition = position;
            this.position = position;

            LoadContent();
        }

        private Animation shotAnimation;

        public void LoadContent()
        {
            texture = Level.Content.Load<Texture2D>("Sprites/Shot");
            
            shotAnimation = new Animation(Level.Content.Load<Texture2D>("Sprites/Shot"), 0.1f, false);
            sprite.PlayAnimation(shotAnimation);

            origin = new Vector2(texture.Width / 2.0f, texture.Height / 2.0f);
            shotSound = Level.Content.Load<SoundEffect>("Sounds/GemCollected"); //REPLACE THIS WITH STEPHS SOUND FILE
        }

        public void Update(GameTime gameTime)
        {
            float t = (float)gameTime.ElapsedGameTime.TotalSeconds;

            Vector2 v = new Vector2(velocity * t, 0.0f);
            position = position + v;
        }

        public void OnShot()
        {
            shotSound.Play();
        }

        /*
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, Vector2 screen)
        {
            spriteBatch.Draw(texture, position - screen, null, Color, 0.0f, origin, 1.0f, SpriteEffects.None, 0.0f);
        }
        */
        
        
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, Vector2 screen)
        {
            // Draw facing the way the enemy is moving.
            //7SpriteEffects flip = direction > 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            sprite.Draw(gameTime, spriteBatch, Position - screen, SpriteEffects.None);
        }


    }
}
