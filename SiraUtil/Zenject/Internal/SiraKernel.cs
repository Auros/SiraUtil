using SiraUtil.Logging;
using System;
using System.Collections.Generic;
using Zenject;

namespace SiraUtil.Zenject.Internal
{
    internal class SiraKernel : IInitializable
    {
        private readonly SiraLog _siraLog;
        private readonly List<IAsyncInitializable> _asyncInitializables;

        public SiraKernel(SiraLog siraLog, [Inject(Optional = true, Source = InjectSources.Local)] List<IAsyncInitializable> asyncInitializables)
        {
            _siraLog = siraLog;
            _asyncInitializables = asyncInitializables;
        }
        public void Initialize()
        {
            foreach (var initter in _asyncInitializables)
            {
                try
                {
                    initter.InitializeAsync();
                }
                catch (Exception e)
                {
                    _siraLog.Error($"Error while asynchronously initializing {initter.GetType().Name}");
                    _siraLog.Critical(e);
                }
            }
        }
    }
}