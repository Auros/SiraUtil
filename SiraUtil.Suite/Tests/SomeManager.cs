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

        public void Initialize()
        {
            _siraLog.Info($"Created {nameof(SomeManager)}");
        }
    }

    public interface ISomeManager
    {

    }
}