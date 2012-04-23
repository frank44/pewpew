#region File Description
//-----------------------------------------------------------------------------
// Level.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using System.IO;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Eve
{
    /// <summary>
    /// A uniform grid of tiles with collections of gems and enemies.
    /// The level owns the player and controls the game's win and lose
    /// conditions as well as scoring.
    /// </summary>
    class Level : IDisposable
    {

        //Screen position relative to entire level.
        private Vector2 screen;
        //The width and height of the level in terms of pixels.
        public Vector2 levelDimensions;
        //Information on the window such as height and width (i.e. resolution).
        private Viewport window;

        // Physical structure of the level.
        private Tile[,] tiles;
        private Background background;

        // Entities in the level.
        public Player Player
        {
            get { return player; }
        }
        Player player;
        
        //FRANK: Made this public so I could modify them elsewhere 
        public List<Gem> gems = new List<Gem>();
        public List<Enemy> enemies = new List<Enemy>();
        public List<Shot> shots = new List<Shot>();
        public List<Object> objects = new List<Object>();
        public TargetDot td;
        public GoldDot gd;

        // Key locations in the level.        
        private Vector2 start;
        private Point exit = InvalidPosition;
        private static readonly Point InvalidPosition = new Point(-1, -1);

        // Level game state.
        private Random random = new Random(128); // Arbitrary, but constant seed

        public int Score
        {
            get { return score; }
        }
        int score;

        public bool ReachedExit
        {
            get { return reachedExit; }
        }
        bool reachedExit;

        // Level content.        
        public ContentManager Content
        {
            get { return content; }
        }
        ContentManager content;

        private SoundEffect exitReachedSound;

        #region Loading


        /// <summary>
        /// Constructs a level from the statistics in the current session.
        /// </summary>
        public Level(ContentManager content, Viewport windowData)
        {
            string levelPath = string.Format("Content/Levels/{0}.txt", Session.StatisticsManager.LevelIndex);
            Stream fileStream = TitleContainer.OpenStream(levelPath);

            // Create a new content manager to load content used just by this level.
            this.content = content;
            window = windowData;

            LoadTiles(fileStream);

            // Load the objects from the object file for the level.
            levelPath = string.Format("Content/Levels/{0}_objects.txt", Session.StatisticsManager.LevelIndex);
            fileStream = TitleContainer.OpenStream(levelPath);
            LoadObjects(fileStream);

            //Set the current start position of the player to the one provided by the statisticsManager if it exists
            if (Session.StatisticsManager.Position.X >= 0 && Session.StatisticsManager.Position.Y >= 0)
            {
                start = Session.StatisticsManager.Position;
            }

            // Set the player to start at the specified position.
            Player.Level = this;

            // Whenever a level is loaded, make sure to update the enemies list of the last save and the position to start.
            Session.LastSavedStats.UpdateEnemies(enemies);
            Session.LastSavedStats.UpdateObjects(objects);
            Session.LastSavedStats.SetPosition(start);
            
            Player.Reset(start);

            td = new TargetDot(this);

            levelDimensions = new Vector2(Width, Height) * Tile.Size;

            // Load background layer textures.
            background = new Background(content, Session.StatisticsManager.LevelIndex);

            
            // Load sounds.
            try
            {
                MediaPlayer.IsRepeating = true;
                MediaPlayer.Play(content.Load<Song>(string.Format("Sounds/Level{0}", Session.StatisticsManager.LevelIndex)));
            }
            catch { }
            exitReachedSound = Content.Load<SoundEffect>("Sounds/ExitReached");
            
            fileStream.Close();
        }


        /// <summary>
        /// Iterates over every tile in the structure file and loads its
        /// appearance and behavior. This method also validates that the
        /// file is well-formed with a player start point, exit, etc.
        /// </summary>
        /// <param name="fileStream">
        /// A stream containing the tile data.
        /// </param>
        private void LoadTiles(Stream fileStream)
        {
            // Load the level and ensure all of the lines are the same length.
            int width;
            List<string> lines = new List<string>();
            using (StreamReader reader = new StreamReader(fileStream))
            {
                string line = reader.ReadLine();
                width = line.Length;
                while (line != null)
                {
                    lines.Add(line);
                    if (line.Length != width)
                        throw new Exception(String.Format("The length of line {0} is different from all preceeding lines.", lines.Count));
                    line = reader.ReadLine();
                }
            }

            // Allocate the tile grid.
            tiles = new Tile[width, lines.Count];

            // Loop over every tile position,
            for (int y = 0; y < Height; ++y)
            {
                for (int x = 0; x < Width; ++x)
                {
                    // to load each tile.
                    char tileType = lines[y][x];
                    tiles[x, y] = LoadTile(tileType, x, y);
                }
            }

            // Verify that the level has a beginning and an end.
            if (Player == null)
                throw new NotSupportedException("A level must have a starting point.");
            if (exit == InvalidPosition)
                throw new NotSupportedException("A level must have an exit.");

        }

        /// <summary>
        /// Loads an individual tile's appearance and behavior.
        /// </summary>
        /// <param name="tileType">
        /// The character loaded from the structure file which
        /// indicates what should be loaded.
        /// </param>
        /// <param name="x">
        /// The X location of this tile in tile space.
        /// </param>
        /// <param name="y">
        /// The Y location of this tile in tile space.
        /// </param>
        /// <returns>The loaded tile.</returns>
        private Tile LoadTile(char tileType, int x, int y)
        {
            Vector2 position;
            switch (tileType)
            {
                // Blank space
                case '.':
                    return new Tile(null, TileCollision.Passable);

                // Exit
                case 'X':
                    return LoadExitTile(x, y);

                // Gem
                case 'G':
                    return LoadGemTile(x, y);

                // Floating platform
                case '-':
                    return LoadTile("Platform", TileCollision.Platform);

                // Transparent block
                case ',':
                    return new Tile(null, TileCollision.Platform);

                // Various enemies
                case 'A':
                    position = RectangleExtensions.GetBottomCenter(GetBounds(x, y));
                    enemies.Add(new TB(this, position));
                    return new Tile(null, TileCollision.Passable);
                case 'B':
                    position = RectangleExtensions.GetBottomCenter(GetBounds(x, y));
                    enemies.Add(new Malaria(this, position));
                    return new Tile(null, TileCollision.Passable);
                case 'C':
                    position = RectangleExtensions.GetBottomCenter(GetBounds(x, y));
                    enemies.Add(new HIV(this, position));
                    return new Tile(null, TileCollision.Passable);
                case 'D':
                    //return LoadEnemyTile(x, y, "MonsterD");

                // Platform block
                case '~':
                    return LoadVarietyTile("BlockB", 2, TileCollision.Platform);

                // Passable block
                case ':':
                    return LoadVarietyTile("BlockB", 2, TileCollision.Passable);

                // Player 1 start point
                case '1':
                    return LoadStartTile(x, y);

                // Impassable block
                case '#':
                    return LoadVarietyTile("BlockA", 7, TileCollision.Impassable);

                // Unknown tile type character
                default:
                    throw new NotSupportedException(String.Format("Unsupported tile type character '{0}' at position {1}, {2}.", tileType, x, y));
            }
        }

        /// <summary>
        /// Creates a new tile. The other tile loading methods typically chain to this
        /// method after performing their special logic.
        /// </summary>
        /// <param name="name">
        /// Path to a tile texture relative to the Content/Tiles directory.
        /// </param>
        /// <param name="collision">
        /// The tile collision type for the new tile.
        /// </param>
        /// <returns>The new tile.</returns>
        private Tile LoadTile(string name, TileCollision collision)
        {
            return new Tile(Content.Load<Texture2D>("Tiles/" + name), collision);
        }


        /// <summary>
        /// Loads a tile with a random appearance.
        /// </summary>
        /// <param name="baseName">
        /// The content name prefix for this group of tile variations. Tile groups are
        /// name LikeThis0.png and LikeThis1.png and LikeThis2.png.
        /// </param>
        /// <param name="variationCount">
        /// The number of variations in this group.
        /// </param>
        private Tile LoadVarietyTile(string baseName, int variationCount, TileCollision collision)
        {
            int index = random.Next(variationCount);
            return LoadTile(baseName + index, collision);
        }


        /// <summary>
        /// Instantiates a player, puts him in the level, and remembers where to put him when he is resurrected.
        /// </summary>
        private Tile LoadStartTile(int x, int y)
        {
            if (Player != null)
                throw new NotSupportedException("A level may only have one starting point.");

            start = RectangleExtensions.GetBottomCenter(GetBounds(x, y));
            player = new Player(this, start);

            return new Tile(null, TileCollision.Passable);
        }

        /// <summary>
        /// Remembers the location of the level's exit.
        /// </summary>
        private Tile LoadExitTile(int x, int y)
        {
            if (exit != InvalidPosition)
                throw new NotSupportedException("A level may only have one exit.");

            exit = GetBounds(x, y).Center;

            return LoadTile("Exit", TileCollision.Passable);
        }

        /// <summary>
        /// Instantiates an enemy and puts him in the level.
        /// </summary>
        /// 
        /*
        private Tile LoadEnemyTile(int x, int y, string spriteSet)
        {
            Vector2 position = RectangleExtensions.GetBottomCenter(GetBounds(x, y));
            enemies.Add(new Enemy(this, position));

            return new Tile(null, TileCollision.Passable);
        }
        */
        /// <summary>
        /// Instantiates a gem and puts it in the level.
        /// </summary>
        private Tile LoadGemTile(int x, int y)
        {
            Point position = GetBounds(x, y).Center;
            gems.Add(new Gem(this, new Vector2(position.X, position.Y)));

            return new Tile(null, TileCollision.Passable);
        }


        private void LoadObjects(Stream fileStream)
        {
            using (StreamReader reader = new StreamReader(fileStream))
            {
                string objectID = reader.ReadLine();
                while (objectID != null)
                {
                    string typeLine = reader.ReadLine();
                    string[] objectInfo = ObjectManager.getObjectInfo(typeLine);
                    string[] positionLine = reader.ReadLine().Split(' ');
                    Vector2 position = new Vector2(float.Parse(positionLine[0]), float.Parse(positionLine[1]));
                    if (objectInfo[0] == "Trigger")
                    {
                        objects.Add(new TriggerObject(typeLine, position, int.Parse(objectID), objectInfo[1] == "Reversible"));
                    }
                    else if (objectInfo[0] == "ProximityTrigger")
                    {
                        objects.Add(new ProximityTriggerObject(typeLine, position, float.Parse(objectInfo[1]), int.Parse(objectID)));
                    }

                    else if (objectInfo[0] == "Sign")
                    {
                        string fact = reader.ReadLine();
                        objects.Add(new Sign(typeLine, position, fact, int.Parse(objectID)));
                    }
                    else if (objectInfo[0] == "Activating")
                    {
                        string[] objectIDs = reader.ReadLine().Split(' ');
                        objects.Add(new ActivatingObject(typeLine, position, int.Parse(objectID), objectIDs)); 
                    }
                    else
                    {
                        objects.Add(new Object(typeLine, position, int.Parse(objectID)));
                    }
                    objectID = reader.ReadLine();
                }
            }
        }


        /// <summary>
        ///  Updates the screen position based on the current position of the player.
        /// </summary>
        private void UpdateScreen(Vector2 position)
        {
            //Screen would be too far to the left, so fix the screen.
            if (position.X - window.Width / 2.0f <= 0)
                screen.X = 0;
            //Screen would be too far to the right.
            else if (position.X + window.Width / 2.0f >= levelDimensions.X)
                screen.X = levelDimensions.X - window.Width;
            //Otherwise set the screen so that the character is horizontally in the middle.
            else
                screen.X = position.X - window.Width / 2.0f;

            //Screen would be too far down, so fix the screen.
            if (position.Y + window.Height / 2.0f >= levelDimensions.Y)
                screen.Y = levelDimensions.Y - window.Height;
            //Screen would be too far up.
            else if (position.Y - window.Height / 2.0f <= 0)
                screen.Y = 0;
            //Otherwise set the screen so that the character is vertically in the middle.
            else
                screen.Y = position.Y - window.Height / 2.0f;
        }

        /// <summary>
        /// Unloads the level content.
        /// </summary>
        public void Dispose()
        {
            background.Dispose();
            Content.Unload();
        }

        #endregion

        #region Bounds and collision

        /// <summary>
        /// Gets the collision mode of the tile at a particular location.
        /// This method handles tiles outside of the levels boundries by making it
        /// impossible to escape past the left or right edges, but allowing things
        /// to jump beyond the top of the level and fall off the bottom.
        /// </summary>
        public TileCollision GetCollision(int x, int y)
        {
            // Prevent escaping past the level ends.
            if (x < 0 || x >= Width)
                return TileCollision.Impassable;
            // Allow jumping past the level top and falling through the bottom.
            if (y < 0 || y >= Height)
                return TileCollision.Passable;

            return tiles[x, y].Collision;
        }

        /// <summary>
        /// Gets the bounding rectangle of a tile in world space.
        /// </summary>        
        public Rectangle GetBounds(int x, int y)
        {
            return new Rectangle(x * Tile.Width, y * Tile.Height, Tile.Width, Tile.Height);
        }

        /// <summary>
        /// Width of level measured in tiles.
        /// </summary>
        public int Width
        {
            get { return tiles.GetLength(0); }
        }

        /// <summary>
        /// Height of the level measured in tiles.
        /// </summary>
        public int Height
        {
            get { return tiles.GetLength(1); }
        }

        #endregion

        #region Update

        /// <summary>
        /// Updates all objects in the world, performs collision between them,
        /// and handles the time limit with scoring.
        /// </summary>
        public void Update(GameTime gameTime)
        {
            // Pause while the player is dead or time is expired.
            if (!Player.IsAlive)
            {
                // Still want to perform physics on the player.
                Player.ApplyPhysics(gameTime);
            }
            else
            {
                Player.Update(gameTime);
                UpdateScreen(Player.Position);

                foreach (Object currentObject in objects)
                {
                    if (currentObject.ObjectClass == ObjectClass.ProximityTrigger)
                    {
                        if (((ProximityTriggerObject)currentObject).AreaOfTrigger.Intersects(Player.BoundingRectangle))
                        {
                            ((ProximityTriggerObject)currentObject).Trigger();
                        }
                    }
                    else if (currentObject.ObjectClass == ObjectClass.Activate)
                    {
                        if (currentObject.Parts[0].BoundingRectangle.Intersects(Player.BoundingRectangle)
                            && InputManager.IsActionTriggered(InputManager.Action.Activate))
                        {
                            ((ActivatingObject)currentObject).Activate();
                        }

                    }
                    currentObject.Update(gameTime);
                }

                // Falling off the bottom of the level kills the player.
                if (Player.BoundingRectangle.Top >= Height * Tile.Height)
                {
                    OnPlayerKilled(null);
                }

                UpdateEnemies(gameTime);

                int ct = 0;
                foreach (Shot s in shots)
                    ct++;

                UpdateShots(gameTime);

                td.Update(gameTime);

                // The player has reached the exit if they are standing on the ground and
                // his bounding rectangle contains the center of the exit tile. They can only
                // exit when they have collected all of the gems.
                if (Player.IsAlive &&
                    Player.IsOnGround &&
                    Player.BoundingRectangle.Contains(exit))
                {
                    OnExitReached();
                }
            }
        }


        /// <summary>
        /// Animates each enemy and allow them to kill the player.
        /// </summary>
        private void UpdateEnemies(GameTime gameTime)
        {
            for (int i = 0; i < enemies.Count; i++)
            {
                Enemy enemy = enemies[i];
                enemy.Update(gameTime);

                if (enemy is HIV)
                {
                    HIV ee = (HIV)enemy;
                    if (ee.dormant)
                        continue;
                }
                // Touching an enemy instantly kills the player
                
                if (enemy.BoundingRectangle.Intersects(Player.BoundingRectangle))
                    OnPlayerKilled(enemy);
                
                for (int j=0; j<shots.Count; j++)
                    if (shots[j].shotIndex == enemy.killIndex && enemy.BoundingRectangle.Intersects(shots[j].BoundingRectangle))
                    {
                        enemy.OnKilled();
                        if (!enemy.alive)
                            enemies.RemoveAt(i--);
                        
                        shots.RemoveAt(j--);
                        break;
                    }
            }
        }

        private void UpdateShots(GameTime gameTime)
        {
            for (int i = 0; i < shots.Count; i++)
            {
                Shot shot = shots[i];
                //if (s.Position.Y > window.Width) (add this if game starts lagging)
                //    shots.RemoveAt(i--);

                Rectangle bounds = shot.BoundingRectangle;
                int leftTile = (int)Math.Floor((float)bounds.Left / Tile.Width);
                int rightTile = (int)Math.Ceiling(((float)bounds.Right / Tile.Width)) - 1;
                int topTile = (int)Math.Floor((float)bounds.Top / Tile.Height);
                int bottomTile = (int)Math.Ceiling(((float)bounds.Bottom / Tile.Height)) - 1;

                for (int y = topTile; y <= bottomTile; ++y) // For each potentially colliding tile
                    for (int x = leftTile; x <= rightTile; ++x)
                    {
                        TileCollision collision = GetCollision(x, y);
                        if (collision != TileCollision.Passable) // If this tile is collidable
                        {
                            // Determine collision depth (with direction) and magnitude.
                            Rectangle tileBounds = GetBounds(x, y);
                            Vector2 depth = RectangleExtensions.GetIntersectionDepth(bounds, tileBounds);
                            if (depth != Vector2.Zero)
                            {
                                shots.RemoveAt(i);
                                i--;
                                goto skip;
                            }
                        }
                    }
                

                shot.Update(gameTime);
            skip:
                ;

            }
        }

        /// <summary>
        /// Called when a gem is collected.
        /// </summary>
        /// <param name="gem">The gem that was collected.</param>
        /// <param name="collectedBy">The player who collected this gem.</param>
        private void OnGemCollected(Gem gem, Player collectedBy)
        {
            score += Gem.PointValue;

            gem.OnCollected(collectedBy);
        }

        /// <summary>
        /// Called when the player is killed.
        /// </summary>
        /// <param name="killedBy">
        /// The enemy who killed the player. This is null if the player was not killed by an
        /// enemy, such as when a player falls into a hole.
        /// </param>
        private void OnPlayerKilled(Enemy killedBy)
        {
            Player.OnKilled(killedBy);
        }

        /// <summary>
        /// Called when the player reaches the level's exit.
        /// </summary>
        private void OnExitReached()
        {
            Player.OnReachedExit();
            exitReachedSound.Play();
            reachedExit = true;
        }

        /// <summary>
        /// Restores the player to the starting point to try the level again.
        /// </summary>
        public void StartNewLife()
        {
            Player.Reset(Session.LastSavedStats.Position);
            enemies = new List<Enemy>();
            foreach (Enemy enemy in Session.LastSavedStats.Enemies)
            {
                enemies.Add(enemy.Clone());
            }
            objects = new List<Object>();
            foreach (Object currentObject in Session.LastSavedStats.Objects)
            {
                objects.Add(currentObject.Clone());
            }
        }

        #endregion

        #region Draw

        /// <summary>
        /// Draw everything in the level from background to foreground.
        /// </summary>
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, Color color, bool freeze = false)
        {
            background.Draw(spriteBatch, screen, window, color);

            DrawTiles(spriteBatch, color);

            foreach (Object currentObject in objects)
            {
                // Get the new position of the object from the top left corner of the sprite.
                Vector2 newPosition = currentObject.Position - screen - currentObject.Sprite.Origin;

                // Do not draw if out of scope of the window.
                if (newPosition.X + currentObject.Animation.FrameWidth >= 0 && newPosition.X <= window.Width
                    && newPosition.Y + currentObject.Animation.FrameHeight >= 0 && newPosition.Y <= window.Height)
                    currentObject.Draw(gameTime, spriteBatch, screen, color);
            }


            foreach (Enemy enemy in enemies)
            {
                Vector2 newPosition = enemy.Position - screen;
                //Do not draw if out of scope of the window.
                if (newPosition.X >= 0 && newPosition.X <= window.Width
                    && newPosition.Y >= 0 && newPosition.Y <= window.Height)
                    enemy.Draw(gameTime, spriteBatch, color, screen, freeze);
            }

            
            Player.Draw(gameTime, spriteBatch, color, screen, freeze);
            td.Draw(gameTime, spriteBatch, color, screen, freeze);

            if (gd != null)
                gd.Draw(gameTime, spriteBatch, color, screen, freeze);
            

            for (int i = 0; i < shots.Count; i++)
            {
                Shot shot = shots[i];

                //If we reach here, we want to draw this bullet

                Vector2 newPosition = shot.Position - screen;
                //Do not draw if out of scope of the window.

                if (newPosition.X >= 0 && newPosition.X <= window.Width
                    && newPosition.Y >= 0 && newPosition.Y <= window.Height)
                {
                    shot.Draw(gameTime, spriteBatch, color, screen, freeze);
                }
            }
        }


        /// <summary>
        /// Draws each tile in the level.
        /// </summary>
        private void DrawTiles(SpriteBatch spriteBatch, Color color)
        {
            // For each tile position
            for (int y = 0; y < Height; ++y)
            {
                for (int x = 0; x < Width; ++x)
                {
                    // If there is a visible tile in that position
                    Texture2D texture = tiles[x, y].Texture;
                    if (texture != null)
                    {
                        // Draw it in screen space.
                        Vector2 position = new Vector2(x, y) * Tile.Size - screen;
                        if (position.X >= -Tile.Width && position.X <= window.Width
                            && position.Y >= -Tile.Height && position.Y <= window.Height)
                            spriteBatch.Draw(texture, position, color);
                    }
                }
            }
        }

        #endregion
    }
}
