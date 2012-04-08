using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.IO;

namespace Platformer
{
    /// <summary>
    /// Has all the hardcoded values like parts of each object.
    /// </summary>
    public static class ObjectManager
    {
        #region Constants


        /// <summary>
        /// Path to the objects file.
        /// </summary>
        private const string levelPath = @"Content\Objects.txt";

        /// <summary>
        /// Readable names of each object.
        /// </summary>
        private static List<string> objectName;


        /// <summary>
        /// Parts of each object at each frame of its animation.
        /// </summary>
        private static List<Part[][]> parts;


        #endregion


        #region Initialization


        public static void Initialize()
        {
            objectName = new List<string>();
            parts = new List<Part[][]>();

            StreamReader reader = new StreamReader(TitleContainer.OpenStream(levelPath));
            string name = reader.ReadLine();

            while (name != null)
            {
                objectName.Add(name);

                string objectType = reader.ReadLine();

                int frameCount = int.Parse(reader.ReadLine());
                Part[][] curObjectParts = new Part[frameCount][];
                for (int curFrame = 0; curFrame < frameCount; curFrame++)
                {
                    int numParts = int.Parse(reader.ReadLine());
                    curObjectParts[curFrame] = new Part[numParts];
                    for (int curPart = 0; curPart < numParts; curPart++)
                    {
                        string partType = reader.ReadLine();
                        string[] boundingRectangleInfo = reader.ReadLine().Split(' ');
                        Rectangle boundingRectangle = new Rectangle(int.Parse(boundingRectangleInfo[0]),
                                                                    int.Parse(boundingRectangleInfo[1]),
                                                                    int.Parse(boundingRectangleInfo[2]),
                                                                    int.Parse(boundingRectangleInfo[3]));
                        if (partType == "bouncy")
                        {
                            curObjectParts[curFrame][curPart] = new BouncyPart(boundingRectangle);
                        }
                        else if (partType == "damaging")
                        {
                            curObjectParts[curFrame][curPart] = new DamagingPart(boundingRectangle);
                        }
                        else if (partType == "passable")
                        {
                            curObjectParts[curFrame][curPart] = new PassablePart(boundingRectangle);
                        }
                        else if (partType == "platform")
                        {
                            curObjectParts[curFrame][curPart] = new PlatformPart(boundingRectangle);
                        }
                        else if (partType == "solid")
                        {
                            curObjectParts[curFrame][curPart] = new SolidPart(boundingRectangle);
                        }
                    }
                }
                parts.Add(curObjectParts);
                name = reader.ReadLine();
            }

            reader.Close();
        }


        #endregion


        #region Methods


        /// <summary>
        /// Get the parts for the whole animation of an object.
        /// </summary>
        public static Part[][] getParts(string type)
        {
            for (int i = 0; i < objectName.Count; i++)
            {
                if (objectName[i] == type)
                {
                    return parts[i];
                }
            }
            return null;
        }


        #endregion
    }
}
