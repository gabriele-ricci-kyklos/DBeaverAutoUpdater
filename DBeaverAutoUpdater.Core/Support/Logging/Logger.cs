using GenericCore.Support;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace DBeaverAutoUpdater.Core.Support.Logging
{
    public enum LoggingMode { Console, File }
    public enum LoggingSeverity { Debug, Info, Error }

    class LogEntry
    {
        public LoggingSeverity Severity { get; }
        public string Message { get; }
        public Exception Exception { get; }

        public static LogEntry New(string message, LoggingSeverity severity)
        {
            return new LogEntry(severity, message);
        }

        public static LogEntry New(string message, LoggingSeverity severity, Exception exception)
        {
            return new LogEntry(severity, message, exception);
        }

        private LogEntry(LoggingSeverity severity, string message, Exception exception = null)
        {
            message.AssertHasText(nameof(message));

            Severity = severity;
            Message = message;
            Exception = exception;
        }
    }

    public static class Logger
    {
        private static Task _backgroundTask;
        private static ConcurrentQueue<LogEntry> _logQueue;
        private static bool _stopLoggingRequested = false;

        public static LoggingMode Mode { get; private set; }
        public static LoggingSeverity Severity { get; private set; }
        public static string FilePath { get; private set; }
        public static bool UseBackgroundTask { get; private set; }

        static Logger()
        {
            Initialize($@"Logs\{GetCurrentDateString()}.log");
        }

        public static void Initialize(string logFilePath = null, bool useBackgroundTask = false, LoggingSeverity severity = LoggingSeverity.Debug, LoggingMode mode = LoggingMode.File)
        {
            FilePath = logFilePath.IsNullOrBlankString() && mode == LoggingMode.File ? $@"Logs\{GetCurrentDateString()}.log" : logFilePath;
            Severity = severity;
            Mode = mode;
            UseBackgroundTask = useBackgroundTask;

            if (UseBackgroundTask)
            {
                _logQueue = new ConcurrentQueue<LogEntry>();
                _backgroundTask =
                    Task
                        .Run
                        (() =>
                        {
                            while (!_stopLoggingRequested)
                            {
                                if (!_logQueue.TryPeek(out LogEntry lastEntry))
                                {
                                    continue;
                                }

                                LogImpl(lastEntry);
                                _logQueue.TryDequeue(out lastEntry);
                            }
                        });
            }
        }

        private static TextWriter GetTextWriter()
        {
            if (Mode == LoggingMode.Console)
            {
                return Console.Out;
            }

            string dir = Path.GetDirectoryName(FilePath);
            Directory.CreateDirectory(dir);
            FileStream stream = new FileStream(FilePath, FileMode.Append, FileAccess.Write);
            StreamWriter writer = new StreamWriter(stream);
            return writer;
        }

        private static string GetCurrentTimeString()
        {
            return DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
        }

        private static string GetCurrentDateString()
        {
            return DateTime.Now.ToString("yyyy-MM-dd");
        }

        private static void Log(LogEntry entry)
        {
            entry.AssertNotNull(nameof(entry));

            if (entry.Severity < Severity)
            {
                return;
            }

            if (UseBackgroundTask)
            {
                _logQueue.Enqueue(entry);
            }
            else
            {
                LogImpl(entry);
            }
        }

        private static void LogImpl(LogEntry entry)
        {
            using (TextWriter writer = GetTextWriter())
            {
                TextWriter defaultWriter = Console.Out;
                Console.SetOut(writer);

                string timeStr = GetCurrentTimeString();
                Console.WriteLine($"{timeStr} {entry.Severity.ToString()} - {entry.Message}");

                if (entry.Exception.IsNotNull())
                {
                    Console.WriteLine(entry.Exception.ToString());
                }
            }
        }

        public static void Debug(string message)
        {
            Log(LogEntry.New(message, LoggingSeverity.Debug));
        }

        public static void Info(string message)
        {
            Log(LogEntry.New(message, LoggingSeverity.Info));
        }

        public static void Error(string message, Exception ex = null)
        {
            Log(LogEntry.New(message, LoggingSeverity.Error));
        }

        public static void Flush()
        {
            if (!UseBackgroundTask)
            {
                return;
            }

            _stopLoggingRequested = true;
            _backgroundTask.Wait();
        }
        }
}
