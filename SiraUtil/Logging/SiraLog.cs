using IPA.Logging;

namespace SiraUtil.Logging
{
    /// <summary>
    /// A simple multiplexed sub-logger for Dependency Injection
    /// </summary>
    public class SiraLog
    {
        /// <summary>
        /// The logger that's being wrapped around.
        /// </summary>
        public Logger? Logger { get; private set; }

        /// <summary>
        /// Debug Mode will elevate debug logs to info logs.
        /// </summary>
        public bool DebugMode { get; set; }

        internal void Setup(Logger logger, string name, bool debugMode)
        {
            if (name == null)
            {
                return;
            }
            DebugMode = debugMode;
            Logger = logger.GetChildLogger(name);
        }

        /// <summary>
        /// Log with a <see cref="Logger.Level"/> of info.
        /// </summary>
        /// <param name="obj">The object to log.</param>
        public void Info(object obj)
        {
            Logger?.Info(obj.ToString());
        }

        /// <summary>
        /// Log with a <see cref="Logger.Level"/> of warning.
        /// </summary>
        /// <param name="obj">The object to log.</param>
        public void Warn(object obj)
        {
            Logger?.Warn(obj.ToString());
        }

        /// <summary>
        /// Log with a <see cref="Logger.Level"/> of error.
        /// </summary>
        /// <param name="obj">The object to log.</param>
        public void Error(object obj)
        {
            Logger?.Error(obj.ToString());
        }

        /// <summary>
        /// Log with a <see cref="Logger.Level"/> of trace.
        /// </summary>
        /// <param name="obj">The object to log.</param>
        public void Trace(object obj)
        {
            Logger?.Trace(obj.ToString());
        }

        /// <summary>
        /// Log with a <see cref="Logger.Level"/> of notice.
        /// </summary>
        /// <param name="obj">The object to log.</param>
        public void Notice(object obj)
        {
            Logger?.Notice(obj.ToString());
        }

        /// <summary>
        /// Log with a <see cref="Logger.Level"/> of critical.
        /// </summary>
        /// <param name="obj">The object to log.</param>
        public void Critical(object obj)
        {
            Logger?.Critical(obj.ToString());
        }

        /// <summary>
        /// Log with a <see cref="Logger.Level"/> of debug.
        /// </summary>
        /// <param name="obj">The object to log.</param>
        public void Debug(object obj)
        {
            if (DebugMode)
            {
                Info(obj);
            }
            else
            {
                Logger?.Debug(obj.ToString());
            }
        }

        /// <summary>
        /// Quickly perform a null check on an object and log the results.
        /// </summary>
        /// <param name="obj">The object to null check.</param>
        public void Null(object obj)
        {
            Logger?.Info(obj != null ? $"{obj.GetType().Name} is not null." : $"Object is null");
        }
    }
}