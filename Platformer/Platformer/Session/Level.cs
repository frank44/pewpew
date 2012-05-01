#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Audio;
#endregion

namespace Eve
{
    /// <summary>
    /// The level that the player is currently in. All of the interactions between the player and
    /// the enemies and the objects of the level are done in this class.
    /// </summary>
    class Level
    {
        #region Properties


        /// <summary>
        /// The current level the player is on.
        /// </summary>
        private int levelIndex;


        /// <summary>
        /// The current stage the player is on.
        /// </summary>
        private int stageIndex;


        /// <summary>
        /// The camera of the level that focuses on the player.
        /// </summary>
        private Camera camera;


        /// <summary>
        /// The backgrounds of the current stage.
        /// </summary>
        private Background background;


        /// <summary>
        /// The width and height of the current stage in terms of pixels.
        /// </summary>
        private Vector2 dimensions;


        /// <summary>
        /// Information on the window such as height and width (i.e. resolution).
        /// </summary>
        public Viewport window;


        /// <summary>
        /// The starting location of the player at the current stage.
        /// </summary>
        private Vector2 start;


        /// <summary>
        /// The sound that plays when the player reaches the exit.
        /// </summary>
        private SoundEffect exitReachedSound;


        /// <summary>
        /// Physical structure of the level.
        /// </summary>
        private Tile[,] tiles;

        // -------------Public Properties-------------
        public Player Player;
        public List<Enemy> Enemies;
        public List<Shot> Shots;
        public List<Object> Objects;
        public List<HIVShot> EnemyShots;
        public TargetDot td;
        public GoldDot gd;
        public ContentManager Content;
        public bool ReachedExit;
        // -------------------------------------------

        #endregion

        #region Initialization

        /// <summary>
        /// Constructs a level from the given statistics.
        /// </summary>
        public Level(ContentManager content, Viewport windowData, StatisticsManager statistics)
        {
            levelIndex = statistics.LevelIndex;
            stageIndex = statistics.StageIndex;
            Content = content;
            window = windowData;

            LoadContent();

            //Set the current start position of the player to the one provided by the statisticsManager if it exists
            if (statistics.Position.X >= 0 && statistics.Position.Y >= 0)
            {
                start = statistics.Position;
                Session.LastSavedStats.SetPosition(start);
            }
            Player.Reset(start);

            try
            {
                MediaPlayer.IsRepeating = true;
                MediaPlayer.Play(Content.Load<Song>(string.Format("Sounds/Levels/{0}", Session.StatisticsManager.LevelIndex)));
            }
            catch { }
        }


        /// <summary>
        /// Load the contents of the current stage in the level.
        /// </summary>
        public void LoadContent()
        {
            camera = new Camera();
            Enemies = new List<Enemy>();
            Objects = new List<Object>();
            Shots = new List<Shot>();
            EnemyShots = new List<HIVShot>();
            ReachedExit = false;

            // Load the background textures.
            background = new Background(Content, levelIndex, stageIndex);
            dimensions = new Vector2(background.Width, background.Height);

            // Load the tilemap of the level.
            string levelPath = string.Format("Content/Levels/Level{0}/Stage{1}/tilemap.txt", levelIndex, stageIndex);
            Stream fileStream = TitleContainer.OpenStream(levelPath);
            LoadTiles(fileStream);

            // Load the objects from the object file.
            levelPath = string.Format("Content/Levels/Level{0}/Stage{1}/objects.txt", levelIndex, stageIndex);
            fileStream = TitleContainer.OpenStream(levelPath);
            LoadObjects(fileStream);

            // Load the floors of the level.
            levelPath = string.Format("Content/Levels/Level{0}/Stage{1}/floors.txt", levelIndex, stageIndex);
            fileStream = TitleContainer.OpenStream(levelPath);
            LoadFloors(fileStream);

            // Load the enemies from the enemy file for the level.
            levelPath = string.Format("Content/Levels/Level{0}/Stage{1}/enemies.txt", levelIndex, stageIndex);
            fileStream = TitleContainer.OpenStream(levelPath);
            LoadEnemies(fileStream);

            fileStream.Close();

            // Load sounds.
            exitReachedSound = Content.Load<SoundEffect>("Sounds/ExitReached");

            td = new TargetDot(this);

            // Whenever a level is loaded, make sure to update the enemies list of the last save and the position to start.
            Session.LastSavedStats.UpdateEnemies(Enemies);
            Session.LastSavedStats.UpdateObjects(Objects);
            Session.LastSavedStats.SetPosition(start);
        }


        /// <summary>
        /// Iterates over every tile in the structure file and loads its
        /// appearance and behavior. This method also validates that the
        /// file is well-formed with a player start point, exit, etc.
        /// </summary>
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


                // Floating platform
                case '-':
                    return LoadTile("Platform", TileCollision.Platform);

                // Transparent block
                case ',':
                    return new Tile(null, TileCollision.Platform);

                // Various enemies
                case 'A':
                    position = RectangleExtensions.GetBottomCenter(GetBounds(x, y));
                    Enemies.Add(new TB(this, position));
                    return new Tile(null, TileCollision.Passable);
                case 'B':
                    position = RectangleExtensions.GetBottomCenter(GetBounds(x, y));
                    Enemies.Add(new Malaria(this, position));
                    return new Tile(null, TileCollision.Passable);
                case 'C':
                    position = RectangleExtensions.GetBottomCenter(GetBounds(x, y));
                    Enemies.Add(new HIV(this, position));
                    return new Tile(null, TileCollision.Passable);

                // Player 1 start point
                case '1':
                    return LoadStartTile(x, y);

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
        /// Instantiates a player, puts him in the level, and remembers where to put him when he is resurrected.
        /// </summary>
        private Tile LoadStartTile(int x, int y)
        {
            start = RectangleExtensions.GetBottomCenter(GetBounds(x, y));
            if (Player == null)
            {
                Player = new Player(this, start);
            }
            else
            {
                Player.Reset(start);
            }

            return new Tile(null, TileCollision.Passable);
        }


        /// <summary>
        /// Loads objects from the files for each stage of the level.
        /// </summary>
        private void LoadObjects(Stream fileStream)
        {
            using (StreamReader reader = new StreamReader(fileStream))
            {
                string objectID = reader.ReadLine();
                while (objectID != null)
                {
                    string typeLine = reader.ReadLine();
                    List<string> objectInfo = new List<string>(ObjectManager.getObjectInfo(typeLine));
                    string[] positionLine = reader.ReadLine().Split(' ');
                    Vector2 position = new Vector2(float.Parse(positionLine[0]), float.Parse(positionLine[1]));
                    if (objectInfo[0] == "Trigger")
                    {
                        Objects.Add(new TriggerObject(typeLine, position, int.Parse(objectID),
                                    objectInfo.Contains("Reversible"), objectInfo.Contains("Front")));
                    }
                    else if (objectInfo[0] == "ProximityTrigger")
                    {
                        Objects.Add(new ProximityTriggerObject(typeLine, position, float.Parse(objectInfo[1]),
                                    int.Parse(objectID), objectInfo.Contains("Front")));
                    }
                    else if (objectInfo[0] == "Start")
                    {
                        Objects.Add(new Start(typeLine, position, float.Parse(objectInfo[1]),
                                    int.Parse(objectID), objectInfo.Contains("Front")));
                        start = ((Start)Objects[Objects.Count - 1]).StartPoint;
                        Player = new Player(this, start);
                    }
                    else if (objectInfo[0] == "Exit")
                    {
                        Objects.Add(new Exit(typeLine, position, float.Parse(objectInfo[1]),
                                    int.Parse(objectID), objectInfo.Contains("Front")));
                        ((Exit)Objects[Objects.Count - 1]).ExitReached += OnExitReached;
                    }
                    else if (objectInfo[0] == "Sign")
                    {
                        string fact = reader.ReadLine();
                        Objects.Add(new Sign(typeLine, position, fact, int.Parse(objectID)));
                    }
                    else if (objectInfo[0] == "Activating")
                    {
                        string[] objectIDs = reader.ReadLine().Split(' ');
                        Objects.Add(new ActivatingObject(typeLine, position, int.Parse(objectID), objectIDs,
                                    objectInfo.Contains("Front")));
                    }
                    else if (objectInfo[0] == "Dynamic")
                    {
                        Vector2 displacement = new Vector2(int.Parse(objectInfo[1]), int.Parse(objectInfo[2]));
                        Objects.Add(new DynamicObject(typeLine, position, displacement, int.Parse(objectID), 
                                    objectInfo.Contains("Reversible"), objectInfo.Contains("Looping")));
                    }
                    else if (objectInfo[0] == "CyclicDynamic")
                    {
                        Vector2 displacement = new Vector2(int.Parse(objectInfo[1]), int.Parse(objectInfo[2]));
                        Objects.Add(new CyclicDynamicObject(typeLine, position, position, displacement, int.Parse(objectID), 
                                    objectInfo.Contains("Break"), objectInfo.Contains("Reversible"), objectInfo.Contains("Looping")));
                    }
                    else
                    {
                        Objects.Add(new Object(typeLine, position, int.Parse(objectID), objectInfo.Contains("Front")));
                    }
                    objectID = reader.ReadLine();
                }
            }
        }


        /// <summary>
        /// Load the floors of the level. They are added after all the objects are loaded.
        /// </summary>
        private void LoadFloors(Stream fileStream)
        {
            using (StreamReader reader = new StreamReader(fileStream))
            {
                List<Part> parts = new List<Part>();
                string floorInfo = reader.ReadLine();
                while (floorInfo != null)
                {
                    string[] boundingRectangleInfo = floorInfo.Split(' ');
                    Rectangle boundingRectangle = new Rectangle(int.Parse(boundingRectangleInfo[0]),
                                                                int.Parse(boundingRectangleInfo[1]),
                                                                int.Parse(boundingRectangleInfo[2]),
                                                                int.Parse(boundingRectangleInfo[3]));
                    parts.Add(new SolidPart(boundingRectangle));
                    floorInfo = reader.ReadLine();
                }
                Objects.Add(new Floor(parts, Objects.Count));
            }
        }


        /// <summary>
        /// Load the enemies of the level.
        /// </summary>
        private void LoadEnemies(Stream fileStream)
        {
            using (StreamReader reader = new StreamReader(fileStream))
            {
                string enemyID = reader.ReadLine();
                while (enemyID != null)
                {
                    string enemyType = reader.ReadLine();
                    string[] positionLine = reader.ReadLine().Split(' ');
                    Vector2 position = new Vector2(float.Parse(positionLine[0]), float.Parse(positionLine[1]));
                    if (enemyType == "tuberculosis")
                    {
                        Enemies.Add(new TB(this, position));
                    }
                    else if (enemyType == "malaria")
                    {
                        Enemies.Add(new Malaria(this, position));
                    }

                    else if (enemyType == "hiv")
                    {
                        Enemies.Add(new HIV(this, position));
                    }
                    enemyID = reader.ReadLine();
                }
            }
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
                camera.Update(gameTime, Player.Position, dimensions, window);

                foreach (Object currentObject in Objects)
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

                UpdateShots(gameTime);

                UpdateEnemyShots(gameTime);

                td.Update(gameTime);
            }
        }


        /// <summary>
        /// Animates each enemy and allow them to kill the player.
        /// </summary>
        private void UpdateEnemies(GameTime gameTime)
        {
            for (int i = 0; i < Enemies.Count; i++)
            {
                Enemy enemy = Enemies[i];
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

                for (int j = 0; j < Shots.Count; j++)
                    if (Shots[j].shotIndex == enemy.killIndex && enemy.BoundingRectangle.Intersects(Shots[j].BoundingRectangle))
                    {
                        enemy.OnKilled();
                        if (!enemy.alive)
                            Enemies.RemoveAt(i--);

                        Shots.RemoveAt(j--);
                        break;
                    }
            }
        }

        /// <summary>
        /// Animates each enemy shot and allow them to kill the player.
        /// </summary>
        private void UpdateEnemyShots(GameTime gameTime)
        {
            for (int i = 0; i < EnemyShots.Count; i++)
            {
                HIVShot s = EnemyShots[i];

                if (s.Position.Y > window.Height || s.Position.Y < 0) //removes shots that go under or over the field
                    Shots.RemoveAt(i--);

                // Touching an HIVShot instantly kills the player
                if (s.BoundingRectangle.Intersects(Player.BoundingRectangle))
                    OnPlayerKilled(null);

                //check for collisions with tiled objects
                int leftTile = (int)Math.Floor((float)s.BoundingRectangle.Left / Tile.Width);
                int rightTile = (int)Math.Ceiling(((float)s.BoundingRectangle.Right / Tile.Width)) - 1;
                int topTile = (int)Math.Floor((float)s.BoundingRectangle.Top / Tile.Height);
                int bottomTile = (int)Math.Ceiling(((float)s.BoundingRectangle.Bottom / Tile.Height)) - 1;

                for (int y = topTile; y <= bottomTile; ++y) // For each potentially colliding tile
                    for (int x = leftTile; x <= rightTile; ++x)
                    {
                        TileCollision collision = GetCollision(x, y);
                        if (collision != TileCollision.Passable) // If this tile is collidable
                        {
                            // Determine collision depth (with direction) and magnitude.
                            Rectangle tileBounds = GetBounds(x, y);
                            Vector2 depth = RectangleExtensions.GetIntersectionDepth(s.BoundingRectangle, tileBounds);
                            if (depth != Vector2.Zero)
                            {
                                EnemyShots.RemoveAt(i);
                                i--;
                                goto skip;
                            }
                        }
                    }

                //check for collisions with nontiled objects
                foreach (Object o in Objects)
                    foreach (Part p in o.Parts)
                        if (p.PartType == PartType.Solid)
                        {
                            Vector2 depth = s.BoundingRectangle.GetIntersectionDepth(p.BoundingRectangle);
                            if (depth != Vector2.Zero)
                            {
                                EnemyShots.RemoveAt(i);
                                i--;
                                goto skip;
                            }
                        }

                s.Update(gameTime);
            skip:
                ;
            }
        }

        private void UpdateShots(GameTime gameTime)
        {
            for (int i = 0; i < Shots.Count; i++)
            {
                Shot shot = Shots[i];
                if (shot.Position.Y > window.Height || shot.Position.Y < 0) //removes shots that go under or over the field
                    Shots.RemoveAt(i--);

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
                                Shots.RemoveAt(i);
                                i--;
                                goto skip;
                            }
                        }
                    }

                foreach (Object o in Objects)
                    foreach (Part p in o.Parts)
                    {
                        if (p.PartType == PartType.Solid)
                        {
                            Vector2 depth = bounds.GetIntersectionDepth(p.BoundingRectangle);
                            if (depth != Vector2.Zero)
                            {
                                Shots.RemoveAt(i);
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
        private void OnExitReached(object sender, EventArgs e)
        {
            Player.OnReachedExit();
            exitReachedSound.Play();
            ReachedExit = true;
        }

        /// <summary>
        /// Restores the player to the starting point to try the level again.
        /// </summary>
        public void StartNewLife()
        {
            Player.Reset(Session.LastSavedStats.Position);
            Enemies = new List<Enemy>();
            foreach (Enemy enemy in Session.LastSavedStats.Enemies)
            {
                Enemies.Add(enemy.Clone());
            }
            Objects = new List<Object>();
            foreach (Object currentObject in Session.LastSavedStats.Objects)
            {
                Objects.Add(currentObject.Clone());
            }
        }

        /// <summary>
        /// Goes to the next stage of the level.
        /// </summary>
        public void GoToNextStage()
        {
            stageIndex++;
            LoadContent();
        }

        #endregion


        #region Draw

        /// <summary>
        /// Draw everything in the level from background to foreground.
        /// </summary>
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, Color color, bool freeze = false)
        {
            background.Draw(spriteBatch, camera.Position, window, color);

            DrawTiles(spriteBatch, color);

            // Draw all objects behind the player first.
            foreach (Object currentObject in Objects)
            {
                if (currentObject.Front == false && currentObject.Animation != null)
                {
                    // Get the new position of the object from the top left corner of the sprite.
                    Vector2 newPosition = currentObject.Position - camera.Position - currentObject.Sprite.Origin;

                    // Do not draw if out of scope of the window.
                    if (newPosition.X + currentObject.Animation.FrameWidth >= 0 && newPosition.X <= window.Width
                        && newPosition.Y + currentObject.Animation.FrameHeight >= 0 && newPosition.Y <= window.Height)
                        currentObject.Draw(gameTime, spriteBatch, camera.Position, color, freeze);
                }
            }

            foreach (HIVShot s in EnemyShots)
            {
                Vector2 newPosition = s.Position - camera.Position - s.sprite.Origin;

                //Do not draw if out of scope of the window.

                if (newPosition.X + s.shotAnimation.FrameWidth >= 0 && newPosition.X <= window.Width
                    && newPosition.Y + s.shotAnimation.FrameHeight >= 0 && newPosition.Y <= window.Height)
                    s.Draw(gameTime, spriteBatch, color, camera.Position, freeze);
            }


            foreach (Enemy enemy in Enemies)
            {
                Vector2 newPosition = enemy.Position - camera.Position - enemy.sprite.Origin;
                //Do not draw if out of scope of the window.
                if (newPosition.X + enemy.idleAnimation.FrameWidth >= 0 && newPosition.X <= window.Width
                    && newPosition.Y + enemy.idleAnimation.FrameHeight >= 0 && newPosition.Y <= window.Height)
                    enemy.Draw(gameTime, spriteBatch, color, camera.Position, freeze);
            }


            Player.Draw(gameTime, spriteBatch, color, camera.Position, freeze);
            td.Draw(gameTime, spriteBatch, color, camera.Position, freeze);

            if (gd != null)
                gd.Draw(gameTime, spriteBatch, color, camera.Position, freeze);


            for (int i = 0; i < Shots.Count; i++)
            {
                Shot shot = Shots[i];

                //If we reach here, we want to draw this bullet

                Vector2 newPosition = shot.Position - camera.Position;
                //Do not draw if out of scope of the window.

                if (newPosition.X >= 0 && newPosition.X <= window.Width
                    && newPosition.Y >= 0 && newPosition.Y <= window.Height)
                {
                    shot.Draw(gameTime, spriteBatch, color, camera.Position, freeze);
                }
            }

            // Draw all objects in front of the player first.
            foreach (Object currentObject in Objects)
            {
                if (currentObject.Front == true && currentObject.Animation != null)
                {
                    // Get the new position of the object from the top left corner of the sprite.
                    Vector2 newPosition = currentObject.Position - camera.Position - currentObject.Sprite.Origin;

                    // Do not draw if out of scope of the window.
                    if (newPosition.X + currentObject.Animation.FrameWidth >= 0 && newPosition.X <= window.Width
                        && newPosition.Y + currentObject.Animation.FrameHeight >= 0 && newPosition.Y <= window.Height)
                        currentObject.Draw(gameTime, spriteBatch, camera.Position, color, freeze);
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
                        Vector2 position = new Vector2(x, y) * Tile.Size - camera.Position;
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
