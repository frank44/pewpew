#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
#endregion

namespace Eve
{
    /// <summary>
    /// Controls the background layer of a stage.
    /// </summary>
    class Background
    {
        #region Properties


        /// <summary>
        /// The images that make up the background of a stage.
        /// </summary>
        private Texture2D[,] background;


        /// <summary>
        /// Number of background layers vertically.
        /// </summary>
        private int height;


        /// <summary>
        /// Number of background layers horizontally.
        /// </summary>
        private int width;


        /// <summary>
        /// The height of the entire background in pixels.
        /// </summary>
        public int Height
        {
            get { return height * background[0, 0].Height; }
        }


        /// <summary>
        /// The width of the entire background in pixels.
        /// </summary>
        public int Width
        {
            get { return width * background[0, 0].Width; }
        }


        #endregion


        #region Initialization


        /// <summary>
        /// Constructs a background based off the current level and stage.
        /// </summary>
        public Background(ContentManager content, int levelIndex, int stageIndex)
        {
            List<string> lines = new List<string>();
            using (StreamReader reader = new StreamReader(string.Format("Content/Levels/Level{0}/Stage{1}/background.txt", levelIndex, stageIndex)))
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
                height = lines.Count;
            }
            background = new Texture2D[width, height];
            for (int y = 0; y < height; ++y)
                for (int x = 0; x < width; ++x)
                    background[x, y] = content.Load<Texture2D>(String.Format("Backgrounds/Levels/Level{0}/{1}", levelIndex, lines[y][x]));
        }


        #endregion


        #region Drawing


        //Draw the background images to the screen
        public void Draw(SpriteBatch spriteBatch, Vector2 cameraPosition, Viewport window, Color color)
        {
            Rectangle screen = new Rectangle(0, 0, window.Width, window.Height);
            for (int y = 0; y < height; ++y)
                for (int x = 0; x < width; ++x)
                {
                    //Only draw the background layers visible on the screen.
                    Texture2D currentBackground = background[x, y];
                    Vector2 position = new Vector2(x, y) * new Vector2(currentBackground.Width, currentBackground.Height) - cameraPosition;
                    Rectangle border = new Rectangle((int)position.X, (int)position.Y, currentBackground.Width, currentBackground.Height);
                    if (screen.Intersects(border))
                        spriteBatch.Draw(currentBackground, position, color);
                }
        }


        #endregion
    }
}
