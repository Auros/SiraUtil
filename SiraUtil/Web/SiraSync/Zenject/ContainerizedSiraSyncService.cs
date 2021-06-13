using System.Threading.Tasks;
using Version = Hive.Versioning.Version;

namespace SiraUtil.Web.SiraSync.Zenject
{
    internal class ContainerizedSiraSyncService : ISiraSyncService
    {
        private ISiraSyncService _siraSyncService = null!;

        public void Setup(ISiraSyncService siraSyncService)
        {
            _siraSyncService = siraSyncService;
        }

        public Task<string?> LatestChangelog()
        {
            return _siraSyncService.LatestChangelog();
        }

        public Task<Version?> LatestVersion()
        {
            return _siraSyncService.LatestVersion();
        }
    }
}