using System;

namespace KoobecaFeedController.BL {
    public class Logger {
        private Logger() { }
        public static Logger Instance { get; } = new Logger();

        public void Error(string msg) {
            if((int)Level <= (int)LogLevel.Error)
              WriteLine("ERROR", msg);
        }

        public void Info(string msg) {
            if ((int)Level <= (int)LogLevel.Info)
                WriteLine("INFO", msg);
        }

        public void Warn(string msg) {
            if ((int)Level <= (int)LogLevel.Warn)
                WriteLine("WARN", msg);
        }

        public void Debug(string msg) {
            if ((int)Level <= (int)LogLevel.Debug)
                WriteLine("DEBUG", msg);
        }

        public LogLevel Level { get; set; } = LogLevel.Info;

        

        private void WriteLine(string label, string msg) {
            Console.WriteLine($"{DateTime.Now}:{label}:{msg}");
        }
    }

    public enum LogLevel
    {
        Debug = 0,
        Info = 1,
        Warn = 2,
        Error = 3
    }
}