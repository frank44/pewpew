
#region File Description
//-----------------------------------------------------------------------------
// Player.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using System.Threading;

namespace Platformer
{
    class Player
    {
        // Animations
        private Animation idleAnimation;
        private Animation runAnimation;
        private Animation jumpAnimation;
        private Animation celebrateAnimation;
        private Animation dieAnimation;
        private Animation dashAnimation;
        private Animation shootingAnimation;
       
        private SpriteEffects flip = SpriteEffects.None;
        private AnimationPlayer sprite;

        // Sounds
        private SoundEffect killedSound;
        private SoundEffect jumpSound;
        private SoundEffect fallSound;
        private SoundEffect shootingSound;
        private SoundEffect dashSound;

        private TimeSpan lastShotTime = TimeSpan.Zero;

        public Level Level
        {
            get { return level; }
        }
        Level level;

        public bool IsAlive
        {
            get { return isAlive; }
        }
        bool isAlive;

        // Physics state
        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }
        Vector2 position;

        private float previousBottom;

        public Vector2 Velocity
        {
            get { return velocity; }
            set { velocity = value; }
        }
        Vector2 velocity;

        // Constants for controling horizontal movement
        private const float MoveAcceleration = 13000.0f;
        private const float MaxMoveSpeed = 1750.0f;
        private const float GroundDragFactor = 0.48f;
        private const float AirDragFactor = 0.58f;

        // Dashing Constants
        private const float DashDuration = 0.75f; //dashing time + cooldown
        private const float DashTime = 0.35f; //dashing time
        private const float DashControlPower = 0.14f; //same as jumping (deprecated)

        // Constants for controlling vertical movement
        private const float MaxJumpTime = 0.35f;
        private const float JumpLaunchVelocity = -3500.0f;
        private const float GravityAcceleration = 3200.0f;
        private const float MaxFallSpeed = 550.0f;
        private const float JumpControlPower = 0.14f;

        // Input configuration
        private const float MoveStickScale = 1.0f;
        private const float AccelerometerScale = 1.5f;
        private const Buttons JumpButton = Buttons.A;

        /// <summary>
        /// Gets whether or not the player's feet are on the ground.
        /// </summary>
        public bool IsOnGround
        {
            get { return isOnGround; }
        }
        bool isOnGround;

        /// <summary>
        /// Current user movement input.
        /// </summary>
        private float movement;

        // Jumping state
        private bool isJumping;
        private bool wasJumping;
        private float jumpTime;

        //Dashing State
        private bool isDashing;
        private bool wasDashing;
        private float dashTime;

        //Crawling State
        private bool isCrawling;

        //Shooting State
        private bool isShooting;
        private bool startedShooting;
        private double waitForShot;
        private double shootingCoolDown = 1.0;
        private bool isLeftShift = false;
        private bool isRightShift = false;
        public double rightStickX;
        public double rightStickY;

        //Controls ammo type: [0,3]
        private int shotIndex;

        private Rectangle localBounds;
        /// <summary>
        /// Gets a rectangle which bounds this player in world space.
        /// </summary>
        public Rectangle BoundingRectangle
        {
            get
            {
                int left = (int)Math.Round(Position.X - sprite.Origin.X) + localBounds.X;
                int top = (int)Math.Round(Position.Y - sprite.Origin.Y) + localBounds.Y;

                int newHeight = localBounds.Height;
                if (isDashing || isCrawling)
                    return new Rectangle(left, top + localBounds.Height / 2, localBounds.Width, localBounds.Height / 2);
                else
                    return new Rectangle(left, top, localBounds.Width, localBounds.Height);
            }
        }

        /// <summary>
        /// Constructors a new player.
        /// </summary>
        public Player(Level level, Vector2 position)
        {
            this.level = level;
            shotIndex = 0; //default ammo

            LoadContent();

            Reset(position);
        }

        /// <summary>
        /// Loads the player sprite sheet and sounds.
        /// </summary>
        public void LoadContent()
        {
            // Load animated textures.
            idleAnimation = new Animation(Level.Content.Load<Texture2D>("Sprites/Player/Eve_idle"), 0.1f, true);
            runAnimation = new Animation(Level.Content.Load<Texture2D>("Sprites/Player/Eve_running"), 0.1f, true);
            jumpAnimation = new Animation(Level.Content.Load<Texture2D>("Sprites/Player/Eve_jumping"), 0.1f, false);
            celebrateAnimation = new Animation(Level.Content.Load<Texture2D>("Sprites/Player/Celebrate"), 0.1f, false);
            dieAnimation = new Animation(Level.Content.Load<Texture2D>("Sprites/Player/Eve_dying"), 0.1f, false);
            dashAnimation = new Animation(Level.Content.Load<Texture2D>("Sprites/Player/Eve_sliding_dash"), 0.1f, false);
            shootingAnimation = new Animation(Level.Content.Load<Texture2D>("Sprites/Player/Eve_shooting"), 0.1f, false);

            // Calculate bounds within texture size.            
            int width = (int)(idleAnimation.FrameWidth * 0.4);
            int left = (idleAnimation.FrameWidth - width) / 2;
            int height = (int)(idleAnimation.FrameWidth * 0.8);
            int top = idleAnimation.FrameHeight - height;
            localBounds = new Rectangle(left, top, width, height);

            // Load sounds.
            shootingSound = Level.Content.Load<SoundEffect>("Sounds/SlingshotFire");
            killedSound = Level.Content.Load<SoundEffect>("Sounds/PlayerKilled");
            jumpSound = Level.Content.Load<SoundEffect>("Sounds/PlayerJump");
            fallSound = Level.Content.Load<SoundEffect>("Sounds/PlayerFall");
            dashSound = Level.Content.Load<SoundEffect>("Sounds/PlayerDash");
        }

        /// <summary>
        /// Resets the player to life.
        /// </summary>
        /// <param name="position">The position to come to life at.</param>
        public void Reset(Vector2 position)
        {
            Position = position;
            Velocity = Vector2.Zero;
            isAlive = true;
            sprite.PlayAnimation(idleAnimation);
        }

        /// <summary>
        /// Handles input, performs physics, and animates the player sprite.
        /// </summary>
        /// <remarks>
        /// We pass in all of the input states so that our game is only polling the hardware
        /// once per frame. We also pass the game's orientation because when using the accelerometer,
        /// we need to reverse our motion when the orientation is in the LandscapeRight orientation.
        /// </remarks>
        public void Update(GameTime gameTime)
        {
            HandleInput();

            ApplyPhysics(gameTime);

            if (IsAlive && IsOnGround)
            {
                if (isDashing)
                {
                    sprite.PlayAnimation(dashAnimation);
                }
                else if (isCrawling)
                {
                    sprite.PlayAnimation(idleAnimation); //CHANGE THIS WHEN ISM FINISHES THE SPRITE
                }
                else if (Math.Abs(Velocity.X) > 0.1f)
                {
                    sprite.PlayAnimation(runAnimation);
                }
                else if (startedShooting)
                {
                    sprite.PlayAnimation(shootingAnimation);
                }
                else
                {
                    if (!isShooting)
                        sprite.PlayAnimation(idleAnimation);

                    if (gameTime.TotalGameTime > lastShotTime + TimeSpan.FromSeconds(.5))
                        isShooting = false;
                }
            }

            // Clear input.
            movement = 0.0f;
            isJumping = false;
        }

        /// <summary>
        /// Gets player horizontal movement and jump commands from input.
        /// </summary>
        private void HandleInput()
        {
            // Get analog horizontal movement.
            movement = InputManager.CurrentGamePadState.ThumbSticks.Left.X * MoveStickScale;
            
            // Ignore small movements to prevent running in place.
            if (Math.Abs(movement) < 0.5f)
                movement = 0.0f;

            // If any digital horizontal movement input is found, override the analog movement.
            if (InputManager.IsActionPressed(InputManager.Action.MoveCharacterLeft))
            {
                movement = -1.0f;
                startedShooting = false;
                isShooting = false;
            }
            else if (InputManager.IsActionPressed(InputManager.Action.MoveCharacterRight))
            {
                movement = 1.0f;
                startedShooting = false;
                isShooting = false;
            }

            // Check if the player wants to jump.
            isJumping = InputManager.IsActionPressed(InputManager.Action.Jump);

            //isCrawling = IsOnGround &&
            //             (keyboardState.IsKeyDown(Keys.S) || keyboardState.IsKeyDown(Keys.Down));

            isDashing = IsOnGround && !isCrawling &&
                        InputManager.IsActionPressed(InputManager.Action.Dash) &&
                        Math.Abs(velocity.X) > 0.1f;

            startedShooting = IsOnGround && movement == 0.0f && !isCrawling && !isDashing &&
                         InputManager.IsActionPressed(InputManager.Action.Shoot);

            if (startedShooting)
            {
                rightStickY = -InputManager.currentGamePadState.ThumbSticks.Right.Y;
                rightStickX = InputManager.currentGamePadState.ThumbSticks.Right.X;
            }

            isLeftShift = InputManager.IsActionPressed(InputManager.Action.LeftShift);
            isRightShift = InputManager.IsActionPressed(InputManager.Action.RightShift);

            if (isLeftShift && isRightShift)
            {
                isLeftShift = false;
                isRightShift = false;
            }

        }


        private void handleShifts(GameTime gameTime)
        {
            if (isLeftShift)
                shotIndex = (shotIndex + 2) % 3; //-1 in modulo 3
            else if (isRightShift)
                shotIndex = (shotIndex + 1) % 3;
        }

        /// <summary>
        /// Updates the player's velocity and position based on input, gravity, etc.
        /// </summary>
        public void ApplyPhysics(GameTime gameTime)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            Vector2 previousPosition = Position;

            // Base velocity is a combination of horizontal movement control and
            // acceleration downward due to gravity.
            velocity.X += movement * MoveAcceleration * elapsed;
            velocity.Y = MathHelper.Clamp(velocity.Y + GravityAcceleration * elapsed, -MaxFallSpeed, MaxFallSpeed);
            
            velocity.X = DoDash(velocity.X, gameTime);
            velocity.X = DoCrawl(velocity.X);
            velocity.Y = DoJump(velocity.Y, gameTime);

            handleShifts(gameTime);
            startShooting(gameTime);
            handleBullets(gameTime);

            // Apply pseudo-drag horizontally.
            if (IsOnGround)
                velocity.X *= GroundDragFactor;
            else
                velocity.X *= AirDragFactor;

            // Prevent the player from running faster than his top speed.            
            velocity.X = MathHelper.Clamp(velocity.X, -MaxMoveSpeed, MaxMoveSpeed);

            // Apply velocity.
            Position += velocity * elapsed;
            Position = new Vector2((float)Math.Round(Position.X), (float)Math.Round(Position.Y));

            // If the player is now colliding with the level, separate them.
            HandleCollisions();

            // If the collision stopped us from moving, reset the velocity to zero.
            if (Position.X == previousPosition.X)
                velocity.X = 0;

            if (Position.Y == previousPosition.Y)
                velocity.Y = 0;
        }

        private void startShooting(GameTime gameTime)
        {
            TimeSpan tot = gameTime.TotalGameTime;
            if (tot < lastShotTime + TimeSpan.FromSeconds(shootingCoolDown))
                startedShooting = false;

            if (!startedShooting) return;

            if (rightStickX < 0)
                flip = SpriteEffects.FlipHorizontally;
            else// if (rightStickX > 0)
                flip = SpriteEffects.None;
            /*
            else
            {
                Console.WriteLine("=======================");
                if (flip == SpriteEffects.FlipHorizontally)
                    rightStickX = -1;
                else rightStickX = -1;
            }
             */

            sprite.PlayAnimation(shootingAnimation);

            startedShooting = false;
            isShooting = true;
            waitForShot = 1.0;
            lastShotTime = tot;
        }

        private void handleBullets(GameTime gameTime)
        {
            if (!isShooting) return;

            if (waitForShot > 0.5)
            {
                waitForShot -= gameTime.ElapsedGameTime.TotalSeconds;
                if (waitForShot <= 0.5)
                {
                    Session.StatisticsManager.IncreaseShotCount();
                    Vector2 pos = new Vector2(position.X + 10, position.Y - 60);
                    shootingSound.Play();

                    Shot b = new Shot(level, pos, shotIndex, rightStickY, rightStickX);
                    Level.shots.Add(b);
                }
            }
        }

        /// <summary>
        /// Calculates the Y velocity accounting for jumping and
        /// animates accordingly.
        /// </summary>
        /// <remarks>
        /// During the accent of a jump, the Y velocity is completely
        /// overridden by a power curve. During the decent, gravity takes
        /// over. The jump velocity is controlled by the jumpTime field
        /// which measures time into the accent of the current jump.
        /// </remarks>
        /// <param name="velocityY">
        /// The player's current velocity along the Y axis.
        /// </param>
        /// <returns>
        /// A new Y velocity if beginning or continuing a jump.
        /// Otherwise, the existing Y velocity.
        /// </returns>
        /// 

        private float DoJump(float velocityY, GameTime gameTime)
        {
            // If the player wants to jump
            if (isJumping)
            {
                // Begin or continue a jump
                if ((!wasJumping && IsOnGround) || jumpTime > 0.0f)
                {
                    if (jumpTime == 0.0f)
                        jumpSound.Play();

                    jumpTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
                    sprite.PlayAnimation(jumpAnimation);
                }

                // If we are in the ascent of the jump
                if (0.0f < jumpTime && jumpTime <= MaxJumpTime)
                {
                    // Fully override the vertical velocity with a power curve that gives players more control over the top of the jump
                    velocityY = JumpLaunchVelocity * (1.0f - (float)Math.Pow(jumpTime / MaxJumpTime, JumpControlPower));
                }
                else
                {
                    // Reached the apex of the jump
                    jumpTime = 0.0f;
                }
            }
            else
            {
                // Continues not jumping or cancels a jump in progress
                jumpTime = 0.0f;
            }
            wasJumping = isJumping;

            return velocityY;
        }

        private float DoCrawl(float velocityX)
        {
            if (isCrawling)
            {
                return velocityX / 1.4f;
            }
            else return velocityX;
        }

        private float DoDash(float velocityX, GameTime gameTime)
        {
            // If the player wants to dash
            if (isDashing && !isCrawling)
            {
                // Begin or continue a dash
                if (!wasDashing || dashTime > 0.0f)
                {
                    if (dashTime == 0.0f)
                        dashSound.Play(); //need to make a NEW sound for this

                    dashTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
                }

                // If we are in the middle of a dash
                if (0.0f < dashTime && dashTime <= DashDuration)
                {
                    if (dashTime < DashTime)
                    {
                        velocityX *= 1.5f;
                    }
                }
                else
                {
                    // Reached the end of the dash
                    dashTime = 0.0f;
                    velocityX = 0.0f; //check this line
                }
            }
            else
            {
                // Continues not dashing or cancels a dash in progress
                dashTime = 0.0f;
            }

            wasDashing = isDashing;

            return velocityX;
        }

        /// <summary>
        /// Detects and resolves all collisions between the player and his neighboring
        /// tiles. When a collision is detected, the player is pushed away along one
        /// axis to prevent overlapping. There is some special logic for the Y axis to
        /// handle platforms which behave differently depending on direction of movement.
        /// </summary>
        private void HandleCollisions()
        {
            // Get the player's bounding rectangle and find neighboring tiles.
            Rectangle bounds = BoundingRectangle;
            int leftTile = (int)Math.Floor((float)bounds.Left / Tile.Width);
            int rightTile = (int)Math.Ceiling(((float)bounds.Right / Tile.Width)) - 1;
            int topTile = (int)Math.Floor((float)bounds.Top / Tile.Height);
            int bottomTile = (int)Math.Ceiling(((float)bounds.Bottom / Tile.Height)) - 1;

            // Reset flag to search for ground collision.
            isOnGround = false;

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

            // Save the new bounds bottom.
            previousBottom = bounds.Bottom;
        }

        /// <summary>
        /// Called when the player has been killed.
        /// </summary>
        /// <param name="killedBy">
        /// The enemy who killed the player. This parameter is null if the player was
        /// not killed by an enemy (fell into a hole).
        /// </param>
        public void OnKilled(Enemy killedBy)
        {
            isAlive = false;

            killedSound.Play();
            sprite.PlayAnimation(dieAnimation);
            
            //erase shots after you die
            Level.shots.Clear();
        }

        /// <summary>
        /// Called when this player reaches the level's exit.
        /// </summary>
        public void OnReachedExit()
        {
            sprite.PlayAnimation(celebrateAnimation);
        }

        /// <summary>
        /// Draws the animated player.
        /// </summary>
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, Color color, Vector2 screen, bool freeze = false)
        {
            // Flip the sprite to face the way we are moving.
            if (Velocity.X < 0)
                flip = SpriteEffects.FlipHorizontally;
            else if (Velocity.X > 0)
                flip = SpriteEffects.None;

            // Draw that sprite.
            sprite.Draw(gameTime, spriteBatch, Position - screen, color, flip, freeze);
        }
    }
}


