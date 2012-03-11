using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using System.IO;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Input;

namespace Platformer
{
    /// <summary>
    /// Controls the background layer of a level.
    /// </summary>
    class Background : IDisposable
    {
        //The images that make up the background of a level.
        private Texture2D[,] background;

        //Number of background layers vertically and horizontally
        private int Height, Width;

        private ContentManager content;

        //Constructs a background based off the current level
        public Background(ContentManager content, int levelIndex)
        {
            List<string> lines = new List<string>();
            using (StreamReader reader = new StreamReader(string.Format("Content/Levels/{0}_background.txt", levelIndex)))
            {
                string line = reader.ReadLine();
                Width = line.Length;
                while (line != null)
                {
                    lines.Add(line);
                    if (line.Length != Width)
                        throw new Exception(String.Format("The length of line {0} is different from all preceeding lines.", lines.Count));
                    line = reader.ReadLine();
                }
                Height = lines.Count;
            }
            background = new Texture2D[Width, Height];
            this.content = content;
            for (int y = 0; y < Height; ++y)
                for (int x = 0; x < Width; ++x)
                    background[x, y] = content.Load<Texture2D>("Backgrounds/Layer" + levelIndex + "_" + lines[y][x]);
        }

        public void Dispose()
        {
            content.Unload();
        }

        //Draw the background images to the screen
        public void Draw(SpriteBatch spriteBatch, Vector2 screenPosition, Viewport window, Color color)
        {
            Rectangle screen = new Rectangle(0, 0, window.Width, window.Height);
            for (int y = 0; y < Height; ++y)
                for (int x = 0; x < Width; ++x)
                {
                    //Only draw the background layers visible on the screen.
                    Texture2D currentBackground = background[x, y];
                    Vector2 position = new Vector2(x, y) * new Vector2(currentBackground.Width, currentBackground.Height) - screenPosition;
                    Rectangle border = new Rectangle((int)position.X, (int)position.Y, currentBackground.Width, currentBackground.Height);
                    if (screen.Intersects(border))
                        spriteBatch.Draw(currentBackground, position, color);
                }
        }
    }
}
