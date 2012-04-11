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
    class GoldDot
    {
        public Level level;
        public Vector2 position; // Position in world space of the bottom center of this object.

        // Animations
        public Animation animation;
        public AnimationPlayer sprite;

        // Constructs a TargetDot.
        public GoldDot() { }
        public GoldDot(Level level, Vector2 pos)
        {
            this.level = level;
            position = pos;
            LoadContent("");
        }

        public void LoadContent(string spriteSet)
        {
            // Load animations
            spriteSet = "Sprites/" + spriteSet + "/";
            animation = new Animation(level.Content.Load<Texture2D>(spriteSet + "GoldDot"), 0.5f, false);
        }

        public void Update(GameTime gameTime)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
        }

        // Draws the animation
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, Color color, Vector2 screen, bool freeze = false)
        {
            sprite.PlayAnimation(animation);
            sprite.Draw(gameTime, spriteBatch, position - screen, color, SpriteEffects.None, freeze);
        }

    }
}
