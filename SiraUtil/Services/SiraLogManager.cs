using IPA.Logging;
using System.Reflection;
using System.Collections.Generic;

namespace SiraUtil.Services
{
    internal class SiraLogManager
    {
        internal struct LoggerContext
        {
            public Logger logger;
            public bool debugMode;

            public LoggerContext(Logger logger, bool defaultToDebugMode)
            {
                this.logger = logger;
                debugMode  = defaultToDebugMode;
            }
        }

        private readonly Dictionary<Assembly, LoggerContext> _loggerAssemblies = new Dictionary<Assembly, LoggerContext>();

        internal void AddLogger(Assembly assembly, Logger logger, bool defaultToDebugMode = false)
        {
            if (!_loggerAssemblies.ContainsKey(assembly))
            {
                _loggerAssemblies.Add(assembly, new LoggerContext(logger, defaultToDebugMode));
            }
        }

        internal LoggerContext LoggerFromAssembly(Assembly assembly)
        {
            return _loggerAssemblies[assembly];
        }
    }
}