﻿using System;
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
                "gurney1",
                "cabinet_tall1",
                "cabinet_tall2",
                "cabinet_short1",
                "hospital_sign1",
                "redcross1",
                "desk1",
                "wheelchair1",
                "bed1",
                "surgical_tools1",
                "surgical_table1",
                "xray_broken_glass1",
                "computer1",
                "ivstand1", 
                "door_sideways1",
                "window1",
                "xray_normal1",
                "xray_broken1",
                "stairs1",
                "stairs2",
                "caution_radiation1",
                "plant1",
                "plant2",
                "door_opening1"                

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
                                  new Rectangle(9,102,78,19), new Rectangle(9,128,78,19), new Rectangle(9,144,78,19),
                                  new Rectangle(4,166,88,37)},
                new Rectangle[] { new Rectangle(22,27,54,68) },
                new Rectangle[] { new Rectangle(17,38,216,57) },
                new Rectangle[] { new Rectangle(3,3,86,43) },
                new Rectangle[] { new Rectangle(33,0,28,95) },
                new Rectangle[] { new Rectangle(28,14,34,81) },
                new Rectangle[] { new Rectangle(1,21,9,170), new Rectangle(1,21,9,170), new Rectangle(1,21,9,170),
                                  new Rectangle(1,21,9,170),new Rectangle(0,0,96, 191) },
                new Rectangle[] { new Rectangle(0,0,383,95) },
                new Rectangle[] { new Rectangle(0,0,95,47) },
                new Rectangle[] { new Rectangle(0,0,95,47) },
                new Rectangle[] { new Rectangle(0,0,63,63) },
                new Rectangle[] { new Rectangle(0,0,63,63) },
                new Rectangle[] { new Rectangle(0,0,95,95) },
                new Rectangle[] { new Rectangle(0,0,95,95) },
                new Rectangle[] { new Rectangle(0,0,95,95) },
                new Rectangle[] { new Rectangle(0,0,95,191) },
            };

        /// <summary>
        /// Characteristics of each object at each frame.
        /// </summary>
        private static readonly string[][] characteristics = 
            {
                new string[] {"bouncy"},
                new string[] {""},
                new string[] {""},
                new string[] {""},
                new string[] {"passable actionable"},
                new string[] {""},
                new string[] {""},
                new string[] {""},
                new string[] {"passable", "passable", "passable", "damage", "damage", "damage", "nodamage"},
                new string[] {"damage"},
                new string[] {""},
                new string[] {"damage passable"},
                new string[] {"passable actionable"},
                new string[] {""},    
                new string[] {"", "", "", "", "passable"},
                /*"window1",
                "xray_normal1",
                "xray_broken1",
                "stairs1",
                "stairs2",
                "caution_radiation1",
                "plant1",
                "plant2",
                "door_opening1"
                */
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
            return new string[1];
        }
    }
}
