using IPA.Logging;

namespace SiraUtil.Tools
{
    /// <summary>
    /// A simple multiplexed sub-logger for Dependency Injection
    /// </summary>
    public class SiraLog
    {
        /// <summary>
        /// The logger that's being wrapped around.
        /// </summary>
        public Logger Logger { get; private set; }

        /// <summary>
        /// Debug Mode will elevate debug logs to info logs.
        /// </summary>
        public bool DebugMode { get; set; }

        internal void Setup(Logger logger, string name)
        {
            if (name == null)
            {
                return;
            }
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
        public void Warning(object obj)
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