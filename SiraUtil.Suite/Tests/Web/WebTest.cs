using SiraUtil.Logging;
using SiraUtil.Web;
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

        public WebTest(SiraLog siraLog, IHttpService httpService)
        {
            _siraLog = siraLog;
            _httpService = httpService;
        }

        public async Task InitializeAsync()
        {
            _siraLog.Info(_httpService.UserAgent!);
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
                _siraLog.Info((await responseBig.ReadAsByteArrayAsync()).Length);
        }

        public void Report(float value)
        {
            _siraLog.Info($"{value:P0}");
        }
    }
}