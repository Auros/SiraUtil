using IPA.Logging;
using SiraUtil.Zenject;
using System.Reflection;
using System.Collections.Generic;

namespace SiraUtil.Services
{
    internal class SiraLogManager
    {
        private readonly ZenjectManager _zenjectManager;
        private readonly Dictionary<Assembly, LoggerContext> _loggerAssemblies = new Dictionary<Assembly, LoggerContext>();

        internal SiraLogManager(ZenjectManager zenjectManager)
        {
            _zenjectManager = zenjectManager;
        }

        internal void AddLogger(Assembly assembly, Logger logger, bool defaultToDebugMode = false)
        {
            if (!_loggerAssemblies.ContainsKey(assembly))
            {
                var zen = _zenjectManager.GetZenjector(assembly);
                _loggerAssemblies.Add(assembly, new LoggerContext(logger, zen.IsSlog || defaultToDebugMode));
            }
        }

        internal LoggerContext LoggerFromAssembly(Assembly assembly)
        {
            return _loggerAssemblies[assembly];
        }

        internal struct LoggerContext
        {
            public Logger logger;
            public bool debugMode;

            public LoggerContext(Logger logger, bool defaultToDebugMode)
            {
                this.logger = logger;
                debugMode = defaultToDebugMode;
            }
        }
    }
}