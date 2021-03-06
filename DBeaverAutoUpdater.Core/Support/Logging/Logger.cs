﻿using GenericCore.Support;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace DBeaverAutoUpdater.Core.Support.Logging
{
    public enum LoggingMode { Console, File, Both }
    public enum LoggingSeverity { Debug, Info, Error }

    class LogEntry
    {
        public DateTime Time { get; }
        public string TimeFormatted => Time.ToString("yyyy-MM-dd HH:mm:ss.fff");
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
            Time = DateTime.Now;
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
            FilePath = logFilePath.IsNullOrBlankString() ? $@"Logs\{GetCurrentDateString()}.log" : logFilePath;
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

        private static IList<TextWriter> GetTextWriterList()
        {
            if (Mode == LoggingMode.Console)
            {
                return Console.Out.AsList();
            }

            string dir = Path.GetDirectoryName(FilePath);
            Directory.CreateDirectory(dir);
            FileStream stream = new FileStream(FilePath, FileMode.Append, FileAccess.Write);
            TextWriter writer = new StreamWriter(stream);
            return Mode == LoggingMode.Both ? new List<TextWriter> { writer, Console.Out } : writer.AsList();
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
            IList<TextWriter> writers = GetTextWriterList();

            Parallel
                .ForEach
                (
                    writers,
                    writer =>
                    {
                        using (writer)
                        {
                            writer.WriteLine($"{entry.TimeFormatted} {entry.Severity.ToString()} - {entry.Message}");

                            if (entry.Exception.IsNotNull())
                            {
                                writer.WriteLine(entry.Exception.ToString());
                            }
                        }
                    }
                );
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
            Log(LogEntry.New(message, LoggingSeverity.Error, ex));
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
