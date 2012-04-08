using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Eve
{
    class Logger
    {
        public Logger()
        {
            
        }

        public static void log(string s)
        {
            StreamWriter file = new StreamWriter("test.txt", true);
            file.WriteLine(s);
            file.Close();
        }
    }
}
