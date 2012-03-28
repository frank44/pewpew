using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Platformer
{
    /// <summary>
    /// Has all the hardcoded values like parts of each object.
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
                "door_opening1",
            };

        /// <summary>
        /// Parts of each object at each frame of its animation.
        /// </summary>
        private static readonly Part [][][] parts = 
            {
                new Part[][] { new Part[] {new SolidPart(16,31,156,64)} },
                new Part[][] { new Part[] {new SolidPart(6,4,84,187)} },
                new Part[][] { new Part[] {new SolidPart(5,2,84,189)} },
                new Part[][] { new Part[] {new SolidPart(5,2,182,93)} },
                new Part[][] { new Part[] {new SolidPart(10,14,42,29)} },
                new Part[][] { new Part[] {new SolidPart(44,36,310,59)} },
                new Part[][] { new Part[] {new SolidPart(25,10,337,85)} },
                new Part[][] { new Part[] {new SolidPart(10,35,83,65)} },
                new Part[][] { new Part[] {new SolidPart(9,71,78,27), new SolidPart(9,72,78,27), new SolidPart(9,84,78,23),
                                  new SolidPart(9,102,78,19), new SolidPart(9,128,78,19), new SolidPart(9,144,78,19),
                                  new SolidPart(4,166,88,37)} },
                new Part[][] { new Part[] {new SolidPart(22,27,54,68)} },
                new Part[][] { new Part[] {new SolidPart(17,38,216,57)} },
                new Part[][] { new Part[] {new SolidPart(3,3,86,43)} },
                new Part[][] { new Part[] {new SolidPart(33,0,28,95)} },
                new Part[][] { new Part[] {new SolidPart(28,14,34,81)} },
                new Part[][] { new Part[] {new SolidPart(1,21,9,170), new SolidPart(1,21,9,170), new SolidPart(1,21,9,170),
                                  new SolidPart(1,21,9,170),new SolidPart(0,0,96, 191)} },
                new Part[][] { new Part[] {new SolidPart(0,0,383,95)} },
                new Part[][] { new Part[] {new SolidPart(0,0,95,47)} },
                new Part[][] { new Part[] {new SolidPart(0,0,95,47)} },
                new Part[][] { new Part[] {new SolidPart(0,0,63,63)} },
                new Part[][] { new Part[] {new SolidPart(0,0,63,63)} },
                new Part[][] { new Part[] {new SolidPart(0,0,95,95)} },
                new Part[][] { new Part[] {new SolidPart(0,0,95,95)} },
                new Part[][] { new Part[] {new SolidPart(0,0,95,95)} },
                new Part[][] { new Part[] {new SolidPart(0,0,95,191)} },
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
        /// Get the parts for the whole animation of an object.
        /// </summary>
        public static Part[][] getParts(string type)
        {
            for (int i = 0; i < objectType.Length; i++)
            {
                if (objectType[i] == type)
                {
                    return parts[i];
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
