using Newtonsoft.Json;
using SiraUtil.Logging;
using System;
using System.Threading.Tasks;
using Version = Hive.Versioning.Version;

namespace SiraUtil.Web.SiraSync.Implementations
{
    internal class GitHubSiraSyncService : ISiraSyncService
    {
        private Release? _cachedRelease;
        private string _githubURL = null!;
        private readonly SiraLog _siraLog;
        private readonly IHttpService _httpService;

        public GitHubSiraSyncService(SiraLog siraLog, IHttpService httpService)
        {
            _siraLog = siraLog;
            _httpService = httpService;
        }

        internal void Set(string repoOwner, string repoName)
            => _githubURL = $"https://api.github.com/repos/{repoOwner}/{repoName}/releases";
        
        public async Task<string?> LatestChangelog()
        {
            Release? release = await GetRelease();
            if (release is null)
                return null;
            return release.Body;
        }

        public async Task<Version?> LatestVersion()
        {
            Release? release = await GetRelease();
            if (release is null)
                return null;

            try
            {
                return Version.Parse(release.TagName);
            }
            catch (Exception e)
            {
                _siraLog.Error("Could not convert the tag name into a SemVer version. Make sure your tag name is SemVer!");
                _siraLog.Error(e);
                return null;
            }
        }

        private async Task<Release?> GetRelease()
        {
            if (_cachedRelease is not null)
                return _cachedRelease;

            _siraLog.Debug($"Starting changelog request at {_githubURL}");
            IHttpResponse response = await _httpService.GetAsync(_githubURL);
            if (!response.Successful)
            {
                _siraLog.Error($"({response.Code}) An error occurred while trying to get the latest release. {await response.Error()}");
                return null;
            }

            Release[] releases;
            try
            {
                releases = JsonConvert.DeserializeObject<Release[]>(await response.ReadAsStringAsync());
            }
            catch (Exception e)
            {
                _siraLog.Error("An error occured while trying to deserialize the release body.");
                _siraLog.Error(e);
                return null;
            }
            if (releases.Length == 0)
            {
                _siraLog.Debug($"There are no releases at {_githubURL}");
                return null;
            }
            return _cachedRelease = releases[0];
        }

        private class Release
        {
            [JsonProperty("tag_name")]
            public string TagName { get; set; } = null!;

            [JsonProperty("body")]
            public string Body { get; set; } = null!;
        }
    }
}