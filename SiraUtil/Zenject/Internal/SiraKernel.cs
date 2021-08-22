using IPA.Utilities.Async;
using SiraUtil.Logging;
using System;
using System.Collections.Generic;
using System.Threading;
using Zenject;

namespace SiraUtil.Zenject.Internal
{
    internal class SiraKernel : IInitializable, IDisposable
    {
        private readonly SiraLog _siraLog;
        private readonly List<IAsyncInitializable> _asyncInitializables;
        private readonly CancellationTokenSource _cancellationTokenSource = new();

        public SiraKernel(SiraLog siraLog, [Inject(Optional = true, Source = InjectSources.Local)] List<IAsyncInitializable> asyncInitializables)
        {
            _siraLog = siraLog;
            _asyncInitializables = asyncInitializables;
        }

        public void Initialize()
        {
            foreach (var initter in _asyncInitializables)
            {
                UnityMainThreadTaskScheduler.Factory.StartNew(async () =>
                {
                    try
                    {
                        await initter.InitializeAsync(_cancellationTokenSource.Token);
                    }
                    catch (Exception e)
                    {
                        _siraLog.Error($"Error while asynchronously initializing {initter.GetType().Name}");
                        _siraLog.Critical(e);
                    }
                }, _cancellationTokenSource.Token);
            }
        }

        public void Dispose()
        {
            _cancellationTokenSource.Cancel();
        }
    }
}