using GenericCore;
using GenericCore.Support;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBeaverAutoUpdater.Core.Support.Logging
{
    public static class Logger
    {
        public static LoggingMode Mode { get; private set; }
        public static LoggingSeverity Severity { get; private set; }
        public static string FilePath { get; private set; }

        static Logger()
        {
            Initialize($@"Logs\{GetCurrentDateString()}.log", LoggingSeverity.Debug, LoggingMode.File);
        }

        public static void Initialize(string logFilePath = null, LoggingSeverity severity = LoggingSeverity.Debug, LoggingMode mode = LoggingMode.File)
        {
            FilePath = logFilePath.IsNullOrBlankString() ? $@"Logs\{GetCurrentDateString()}.log" : logFilePath;
            Severity = severity;
            Mode = mode;
        }

        private static TextWriter GetTextWriter()
        {
            if(Mode == LoggingMode.Console)
            {
                return Console.Out;
            }

            FileStream stream = new FileStream(FilePath, FileMode.OpenOrCreate, FileAccess.Write);
            StreamWriter writer = new StreamWriter(stream);
            return writer;
        }

        private static string GetCurrentTimeString()
        {
            return DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
        }

        private static string GetCurrentDateString()
        {
            return DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss-fff");
        }

        private static void Log(string message, LoggingSeverity severity, Exception ex = null)
        {
            if(severity < Severity)
            {
                return;
            }

            using (TextWriter writer = GetTextWriter())
            {
                TextWriter defaultWriter = Console.Out;
                Console.SetOut(writer);

                string timeStr = GetCurrentTimeString();
                Console.WriteLine($"{timeStr} {severity.ToString()} - {message}");

                if(ex.IsNotNull())
                {
                    Console.WriteLine(ex.ToString());
                }
            }
        }

        public static void Debug(string message)
        {
            Log(message, LoggingSeverity.Debug);
        }

        public static void Info(string message)
        {
            Log(message, LoggingSeverity.Info);
        }

        public static void Error(string message, Exception ex = null)
        {
            Log(message, LoggingSeverity.Error, ex);
        }

        public enum LoggingMode { Console, File }
        public enum LoggingSeverity { Debug, Info, Error }
    }
}
