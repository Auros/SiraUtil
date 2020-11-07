using IPA.Logging;

namespace SiraUtil.Tools
{
    /// <summary>
    /// A simple multiplexed sub-logger for Dependency Injection
    /// </summary>
    public class SiraLog
    {
        public Logger Logger { get; private set; }
        public bool DebugMode { get; set; }

        internal void Setup(Logger logger, string name)
        {
            if (name == null)
            {
                return;
            }
            Logger = logger.GetChildLogger(name);
        }

        public void Info(object obj)
        {
            Logger?.Info(obj.ToString());
        }

        public void Warning(object obj)
        {
            Logger?.Warn(obj.ToString());
        }

        public void Error(object obj)
        {
            Logger?.Error(obj.ToString());
        }

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

        public void Null(object obj)
        {
            Logger?.Info(obj != null ? $"{obj.GetType().Name} is not null." : $"Object is null");
        }
    }
}