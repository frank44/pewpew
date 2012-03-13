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
        private Rectangle bounds;

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

        public Circle BoundingCircle
        {
            get
            {
                return new Circle(Position, Tile.Width / 3.0f);
            }
        }

        public Shot(Level level, Vector2 position, SpriteEffects flip, int inx)
        {
            this.shotIndex = inx;
            this.level = level;
            basePosition = position;
            this.position = position;

            if (flip == SpriteEffects.None) //shooting forward
                direction = 1;
            else direction = -1; //shooting back

            time = 0.0f;

            LoadContent();
        }

        public void LoadContent()
        {
            texture = Level.Content.Load<Texture2D>(String.Format("Sprites/vaccine{0}", shotIndex));
            
            shotAnimation = new Animation(texture, 0.1f, false);
            sprite.PlayAnimation(shotAnimation);

            origin = new Vector2(texture.Width / 2.0f, texture.Height / 2.0f);
            shotSound = Level.Content.Load<SoundEffect>("Sounds/SlingshotFire"); 
        }

        public void Update(GameTime gameTime)
        {
            /*
            foreach (Enemy e in Level.enemies)
            {
                ;
            }*/
            /*
            // For each potentially colliding tile,
            for (int y = topTile; y <= bottomTile; ++y)
            {
                for (int x = leftTile; x <= rightTile; ++x)
                {
                    // If this tile is collidable,
                    TileCollision collision = Level.GetCollision(x, y);
                    if (collision != TileCollision.Passable)
                    {
                        // Determine collision depth (with direction) and magnitude.
                        Rectangle tileBounds = Level.GetBounds(x, y);
                        Vector2 depth = RectangleExtensions.GetIntersectionDepth(bounds, tileBounds);
                        if (depth != Vector2.Zero)
                        {
                            float absDepthX = Math.Abs(depth.X);
                            float absDepthY = Math.Abs(depth.Y);

                            // Resolve the collision along the shallow axis.
                            if (absDepthY < absDepthX || collision == TileCollision.Platform)
                            {
                                // If we crossed the top of a tile, we are on the ground.
                                if (previousBottom <= tileBounds.Top)
                                    isOnGround = true;

                                // Ignore platforms, unless we are on the ground.
                                if (collision == TileCollision.Impassable || IsOnGround)
                                {
                                    // Resolve the collision along the Y axis.
                                    Position = new Vector2(Position.X, Position.Y + depth.Y);

                                    // Perform further collisions with the new bounds.
                                    bounds = BoundingRectangle;
                                }
                            }
                            else if (collision == TileCollision.Impassable) // Ignore platforms.
                            {
                                // Resolve the collision along the X axis.
                                Position = new Vector2(Position.X + depth.X, Position.Y);

                                //if (absDepthY > 0.5f) //if a significant y-collision, stop y momentum
                                jumpTime = 0.0f;

                                // Perform further collisions with the new bounds.
                                bounds = BoundingRectangle;
                            }
                        }
                    }
                }
            }
            */

            float t = (float)gameTime.ElapsedGameTime.TotalSeconds;
            time += t;
            Vector2 v = new Vector2(velocity * t * direction, -gravity*time*time);
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
