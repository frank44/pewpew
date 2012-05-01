﻿#region File Description
//-----------------------------------------------------------------------------
// AnimationPlayer.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Eve
{
    /// <summary>
    /// Controls playback of an Animation.
    /// </summary>
    struct AnimationPlayer
    {
        /// <summary>
        /// Gets the animation which is currently playing.
        /// </summary>
        public Animation Animation
        {
            get { return animation; }
        }
        Animation animation;

        /// <summary>
        /// Gets the index of the current frame in the animation.
        /// </summary>
        public int FrameIndex
        {
            get { return frameIndex; }
        }
        public int frameIndex;

        /// <summary>
        /// The amount of time in seconds that the current frame has been shown for.
        /// </summary>
        private float time;

        /// <summary>
        /// Gets a texture origin at the bottom center of each frame.
        /// </summary>
        public Vector2 Origin
        {
            get { return new Vector2(Animation.FrameWidth / 2.0f, Animation.FrameHeight); }
        }

        /// <summary>
        /// The direction in which the sprite advances.
        /// </summary>
        public int direction;

        /// <summary>
        /// Begins or continues playback of an animation.
        /// </summary>
        public void PlayAnimation(Animation animation)
        {
            // If this animation is already running, do not restart it.
            if (Animation == animation)
                return;

            // Start the new animation.
            this.animation = animation;
            this.frameIndex = 0;
            this.time = 0.0f;
            this.direction = 1;
        }

        /// <summary>
        /// Advances the time position and draws the current frame of the animation.
        /// </summary>
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, Vector2 position,
                         Color color, SpriteEffects spriteEffects, bool freeze = false)
        {
            if (Animation == null)
                throw new NotSupportedException("No animation is currently playing.");

            // Process passing time if the sprite is not frozen
            if (!freeze)
            {
                time += (float)gameTime.ElapsedGameTime.TotalSeconds;
                while (time > Animation.FrameTime)
                {
                    time -= Animation.FrameTime;

                    // Advance the frame index; looping or clamping as appropriate.
                    if (Animation.IsLooping)
                    {
                        frameIndex = (frameIndex + direction + Animation.FrameCount) % Animation.FrameCount;
                    }
                    else
                    {
                        frameIndex = Math.Min(Math.Max(0, frameIndex + direction), Animation.FrameCount - 1);
                    }
                }
            }

            // Calculate the source rectangle of the current frame.
            Rectangle source = new Rectangle(FrameIndex * Animation.FrameWidth, 0, Animation.FrameWidth, Animation.FrameHeight);

            // Draw the current frame.
            spriteBatch.Draw(Animation.Texture, position, source, color, 0.0f, Origin, 1.0f, spriteEffects, 0.0f);
        }

        /// <summary>
        /// Advances the time position and draws the current frame of the animation.
        /// Differs from the previous one because it can draw rotated objects
        /// Note: the center of rotation is the center of the sprite
        /// </summary>
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, Vector2 position,
                         Color color, SpriteEffects spriteEffects, float rot, bool freeze = false)
        {
            if (Animation == null)
                throw new NotSupportedException("No animation is currently playing.");

            // Process passing time if the sprite is not frozen
            if (!freeze)
            {
                time += (float)gameTime.ElapsedGameTime.TotalSeconds;
                while (time > Animation.FrameTime)
                {
                    time -= Animation.FrameTime;

                    // Advance the frame index; looping or clamping as appropriate.
                    if (Animation.IsLooping)
                    {
                        frameIndex = (frameIndex + direction + Animation.FrameCount) % Animation.FrameCount;
                    }
                    else
                    {
                        frameIndex = Math.Min(Math.Max(0, frameIndex + direction), Animation.FrameCount - 1);
                    }
                }
            }

            // Calculate the source rectangle of the current frame.
            Rectangle source = new Rectangle(FrameIndex * Animation.FrameWidth, 0, Animation.FrameWidth, Animation.FrameHeight);

            // Draw the current frame.
            spriteBatch.Draw(Animation.Texture, position, source, color, rot, new Vector2(Animation.FrameWidth / 2.0f, Animation.FrameHeight / 2.0f), 1.0f, spriteEffects, 0.0f);
        }
    }
}
