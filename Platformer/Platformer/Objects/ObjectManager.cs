using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Platformer
{
    /// <summary>
    /// Has all the hardcoded values like bounding rectangles of each object.
    /// </summary>
    public static class ObjectManager
    {
        /// <summary>
        /// Readable names of each object.
        /// </summary>
        private static readonly string[] objectType = 
            {
                "gurney",
                "cabinet_tall",
                "cabinet_tall2",
                "cabinet_short",
                "hospital_sign",
                "redcross",
                "desk",
                "wheelchair",
                "bed",
                "surgical_tools",
                "surgical_table",
                "xray_broken_glass",
                "computer",
                "IVstand",
                "window",
                "xray_normal",
                "xray_broken",
                "stairs",
                "caution_radiation",
                "plant1",
                "plant2",
                "door_opening"

            };

        /// <summary>
        /// Bounding rectangles of each object's animation.
        /// </summary>
        private static readonly Rectangle[][] boundingRectangles = 
            {
                new Rectangle[] { new Rectangle(16,31,156,64) },
                new Rectangle[] { new Rectangle(6,4,84,187) },
                new Rectangle[] { new Rectangle(5,2,84,189) },
                new Rectangle[] { new Rectangle(5,2,182,93) },
                new Rectangle[] { new Rectangle(10,14,42,29) },
                new Rectangle[] { new Rectangle(44,36,310,59) },
                new Rectangle[] { new Rectangle(25,10,337,85) },
                new Rectangle[] { new Rectangle(10,35,83,65) },
                new Rectangle[] { new Rectangle(9,71,78,27), new Rectangle(9,72,78,27), new Rectangle(9,84,78,23),
                                  new Rectangle(9,102,78,19), new Rectangle(9,128,78,19), new Rectangle(9,144,78,19) },
                new Rectangle[] { new Rectangle(22,27,54,68) },
                new Rectangle[] { new Rectangle(17,38,216,57) },
                new Rectangle[] { new Rectangle(3,3,86,43) },
                new Rectangle[] { new Rectangle(33,0,28,95) },
                new Rectangle[] { new Rectangle(28,14,34,81) },
                new Rectangle[] { new Rectangle() },
                new Rectangle[] { new Rectangle() },
                new Rectangle[] { new Rectangle() },
                new Rectangle[] { new Rectangle() },
                new Rectangle[] { new Rectangle() },
                new Rectangle[] { new Rectangle() },
                new Rectangle[] { new Rectangle() },
                new Rectangle[] { new Rectangle() },
            };

        /// <summary>
        /// Characteristics of each object at each frame.
        /// </summary>
        private static readonly string[][] characteristics = 
            {
                new string[] {"bouncy"},
                new string[] {"cabinet_tall",
                "cabinet_tall2",
                "cabinet_short",
                "hospital_sign",
                "redcross",
                "desk",
                "wheelchair",
                "bed",
                "surgical_tools",
                "surgical_table",
                "xray_broken_glass",
                "computer",
                "IVstand",
                "window",
                "xray_normal",
                "xray_broken",
                "stairs",
                "caution_radiation",
                "plant1",
                "plant2",
                "door_opening"}

            };


        /// <summary>
        /// Get the bounds for the whole animation of an object.
        /// </summary>
        public static Rectangle[] getBounds(string type)
        {
            for (int i = 0; i < objectType.Length; i++)
            {
                if (objectType[i] == type)
                {
                    return boundingRectangles[i];
                }
            }
            return null;
        }


        /// <summary>
        /// Get the characteristics for the object.
        /// </summary>
        public static string[] getCharacteristics(string type, int frameIndex)
        {
            for (int i = 0; i < objectType.Length; i++)
            {
                if (objectType[i] == type)
                {
                    return characteristics[i][frameIndex].Split(' ');
                }
            }
            return null;
        }
    }
}
