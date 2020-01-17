
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace FB2Koobeca.Utils
{
    public class Logger
    {
        private Logger() { }

        private static Logger _Instance = new Logger();
        public static Logger Instance { get { return _Instance; } }

        
        public void Error(string msg)
        {
            WriteLine("ERROR", msg);
        }

        public void Info(string msg)
        {
            WriteLine("INFO", msg);
        }

        public void Warn(string msg)
        {
            WriteLine("WARN", msg);
        }

        public void Debug(string msg)
        {
            WriteLine("DEBUG", msg);
        }

        private void WriteLine(string label, string msg)
        {
            Console.WriteLine($"{DateTime.Now}:{label}:{msg}");
        }

    }
}
