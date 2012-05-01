using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Eve;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;

namespace Eve
{
    class TargetDot
    {
        public Level level;
        public Vector2 position; // Position in world space of the bottom center of this object.
        public bool invisible;

        // Animations
        public Animation animation;
        public AnimationPlayer sprite;

        // Constructs a TargetDot.
        public TargetDot() { } 
        public TargetDot(Level level)
        {
            this.level = level;
            LoadContent();
        }

        public void LoadContent()
        {
            // Load animations
            animation = new Animation(level.Content.Load<Texture2D>("Sprites/vaccine/vaccine" + level.Player.shotIndex), 0.1f, true);

            //position = level.Player.Position + (new Vector2(10.0f, 60.0f));
        }

        public float angle = 0;

        public void Update(GameTime gameTime)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            float x = (float)level.Player.rightStickX;
            float y = (float)level.Player.rightStickY;

            angle = (float)Math.Atan2(y, x);
            animation = new Animation(level.Content.Load<Texture2D>("Sprites/vaccine/vaccine" + level.Player.shotIndex), 0.1f, true);

            float mag = (float)Math.Sqrt(x * x + y * y);
            invisible =  (mag == 0);
            
            if (invisible) return;

            Vector2 delta = new Vector2(60*x/mag + 10, 60*y/mag - 60);
            position = level.Player.Position + delta;
        }

        // Draws the animation
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, Color color, Vector2 screen, bool freeze = false)
        {
            if (invisible) return;

            sprite.PlayAnimation(animation);
            sprite.Draw(gameTime, spriteBatch, position - screen, color, SpriteEffects.None, angle, freeze);
        }

    }
}
