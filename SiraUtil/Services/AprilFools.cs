using System;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using IPA.Utilities;
using UnityEngine;
using Zenject;
using Random = System.Random;

namespace SiraUtil.Services
{
    internal class AprilFools : IInitializable, IDisposable
    {
        public static int maxInSession = 10;
        public static int sessionTrack = 0;

        private int _max = 8100;
        private readonly Random _random;
        private readonly ScoreController _scoreController;
        private readonly IPlatformUserModel _platformUserModel;
        private readonly CachedMediaAsyncLoader _cachedMediaAsyncLoader;
        private readonly AudioTimeSyncController _audioTimeSyncController;
        private static readonly FileInfo _file = new FileInfo(Path.Combine(UnityGame.UserDataPath, "SIRA", "reaxt.ogg"));
        private AudioSource _audioSource;

        public AprilFools(ScoreController scoreController, IPlatformUserModel platformUserModel, CachedMediaAsyncLoader cachedMediaAsyncLoader, AudioTimeSyncController audioTimeSyncController)
        {
            _random = new Random();
            _scoreController = scoreController;
            _platformUserModel = platformUserModel;
            _cachedMediaAsyncLoader = cachedMediaAsyncLoader;
            _audioTimeSyncController = audioTimeSyncController;
        }

        public async void Initialize()
        {
            _scoreController.noteWasCutEvent += ScoreController_noteWasCutEvent;
            if ((await _platformUserModel.GetUserInfo()).platformUserId == "76561198135839914")
            {
                // If you are Chibiki. Chibiki asked for this in VRC. I don't know why
                _max = 166;
            }
        }

        private void ScoreController_noteWasCutEvent(NoteData noteData, in NoteCutInfo noteCutInfo, int multiplier)
        {
            if (_random.Next(1, _max) == 1)
            {
                sessionTrack++;
                _ = Reaxt();
            }
        }

        private async Task Reaxt()
        {
            if (!_file.Directory.Exists)
            {
                _file.Directory.Create();
            }
            if (!_file.Exists)
            {
                using (var manifestStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("SiraUtil.reaxt.ogg"))
                {
                    using (var fs = File.Create(_file.FullName))
                    {
                        await manifestStream.CopyToAsync(fs);
                    }
                }
            }
            if (_file.Exists)
            {
                await Utilities.AwaitSleep(_random.Next(311, 1004));
                var audioClip = await _cachedMediaAsyncLoader.LoadAudioClipAsync(_file.FullName, CancellationToken.None);
                if (audioClip != null)
                {
                    if (_audioSource == null)
                    {
                        _audioSource = new GameObject("SiraUtil AudioSource").AddComponent<AudioSource>();
                        _audioSource.outputAudioMixerGroup = _audioTimeSyncController.audioSource.outputAudioMixerGroup;
                    }
                    _audioSource.PlayOneShot(audioClip, 1f);
                }
            }

        }

        public void Dispose()
        {
            _scoreController.noteWasCutEvent -= ScoreController_noteWasCutEvent;
        }
    }
}