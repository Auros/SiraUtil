using IPA.Logging;
using SiraUtil.Attributes;
using SiraUtil.Logging;
using SiraUtil.Zenject;
using Zenject;

namespace SiraUtil.Suite.Tests
{
    [Bind(Location.StandardPlayer)]
    public class SomeManager : ISomeManager, IInitializable
    {
        [Inject]
        private readonly SiraLog _siraLog = null!;

        [Inject]
        public void Init(Logger ipaLogger)
        {
            _siraLog.Debug("SomeManager inject method");
            ipaLogger.Debug("SomeManager inject method but logged with IPA logger");
        }

        public void Initialize()
        {
            _siraLog.Info($"Created {nameof(SomeManager)}");
        }
    }

    public interface ISomeManager
    {

    }
}