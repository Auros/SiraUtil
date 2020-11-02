using System;
using Zenject;
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
        private readonly DiContainer _container;

        internal SiraLogManager(DiContainer container)
        {
            _container = container;
        }

        internal void AddLogger(Assembly assembly, Logger logger, bool defaultToDebugMode = false)
        {
            _loggerAssemblies.TryAdd(assembly, new LoggerContext(logger, defaultToDebugMode));
        }

        internal LoggerContext LoggerFromAssembly(Assembly assembly)
        {
            return _loggerAssemblies[assembly];
        }
    }
}