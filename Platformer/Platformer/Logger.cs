using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Platformer
{
    class Logger
    {
        System.IO.StreamWriter file;
        string path;
        public Logger()
        {
            path = System.Environment.GetEnvironmentVariable("TEMP");
        }

        public void log(string s)
        {
            file = new System.IO.StreamWriter("test.txt", true);
            file.WriteLine(s);
            file.Close();
        }
    }
}
