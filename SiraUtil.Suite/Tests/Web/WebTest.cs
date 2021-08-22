using SiraUtil.Logging;
using SiraUtil.Web;
using SiraUtil.Web.SiraSync;
using SiraUtil.Zenject;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SiraUtil.Suite.Tests.Web
{
    internal class WebTest : IAsyncInitializable, IProgress<float>
    {
        private readonly SiraLog _siraLog;
        private readonly IHttpService _httpService;
        private readonly ISiraSyncService _siraSyncService;

        public WebTest(SiraLog siraLog, IHttpService httpService, ISiraSyncService siraSyncService)
        {
            _siraLog = siraLog;
            _httpService = httpService;
            _siraSyncService = siraSyncService;
        }

        public async Task InitializeAsync(CancellationToken token)
        {
            _siraLog.Notice("Testing Changelog:");
            var changelog = await _siraSyncService.LatestChangelog();
            if (changelog != null)
                _siraLog.Info(changelog);

            _siraLog.Notice("Testing Version:");
            var version = await _siraSyncService.LatestVersion();
            if (version != null)
                _siraLog.Info(version);

            /*_siraLog.Info(_httpService.UserAgent!);
            var response = await _httpService.GetAsync("https://suit.auros.dev/api/discord/user/173241614192476161");
            _siraLog.Info(response.Successful);
            _siraLog.Info(response.Code);
            if (response.Successful)
                _siraLog.Info(await response.ReadAsStringAsync());

            _siraLog.Notice("Now testing a big boy file");
            CancellationTokenSource cts = new CancellationTokenSource();
            _ = Task.Run(async () =>
            {
                await Task.Delay(3000);
                cts.Cancel();
            });
            var responseBig = await _httpService.GetAsync("https://bun.auros.dev/toad.gif", this);
            _siraLog.Info(responseBig.Successful);
            _siraLog.Info(responseBig.Code);
            if (responseBig.Successful)
                _siraLog.Info((await responseBig.ReadAsByteArrayAsync()).Length);*/
        }

        public void Report(float value)
        {
            _siraLog.Info($"{value:P0}");
        }
    }
}