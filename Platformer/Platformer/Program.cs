#region File Description
//-----------------------------------------------------------------------------
// Program.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

using System;

namespace Platformer
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            System.IO.StreamWriter file;

            file = new System.IO.StreamWriter("test.txt");
            String s = "Hello World";
            file.WriteLine(s);
            file.Close();


            
            using (PlatformerGame game = new PlatformerGame())
            {
                game.Run();
            }
        }
    }
}

