using System;
using Zenject;
using SiraUtil.Sabers;
using SiraUtil.Interfaces;
using System.Collections.Generic;

namespace SiraUtil.Services
{
	/// <summary>
	/// Handles the processing and change of saber effects.
	/// </summary>
	public class SiraSaberEffectManager : IInitializable, IDisposable
    {
        private bool _safeReady = false;
		private readonly IGamePause _gamePause;
        private readonly Queue<Saber> _temporaryQueue = new Queue<Saber>();
        private readonly Queue<Saber> _temporaryColorQueue = new Queue<Saber>();
        private readonly List<ISaberRegistrar> _saberManagers = new List<ISaberRegistrar>();
		private readonly List<Saber> _managedSabers = new List<Saber>();
		private readonly SiraSaberClashChecker _siraSaberClashChecker;

		public SiraSaberEffectManager(IGamePause gamePause, SaberManager saberManager, SaberClashChecker saberClashChecker, SaberBurnMarkArea saberBurnMarkArea,
									  SaberBurnMarkSparkles saberBurnMarkSparkles, ObstacleSaberSparkleEffectManager obstacleSaberSparkleEffectManager)
		{
			_gamePause = gamePause;
			saberClashChecker.Init(saberManager);
			_saberManagers.Add(saberClashChecker as SiraSaberClashChecker);
			_saberManagers.Add(saberBurnMarkArea as SiraSaberBurnMarkArea);
			_saberManagers.Add(saberBurnMarkSparkles as SiraSaberBurnMarkSparkles);
			_saberManagers.Add(obstacleSaberSparkleEffectManager as SiraObstacleSaberSparkleEffectManager);

			_siraSaberClashChecker = saberClashChecker as SiraSaberClashChecker;
		}

		/// <summary>
		/// Replaces the references of each saber to a new set of sabers for all effect systems.
		/// </summary>
		/// <param name="left"></param>
		/// <param name="right"></param>
		/// <param name="saberManager"></param>
		public void RepatchDefault(Saber left, Saber right, SaberManager saberManager)
		{
			Accessors.SMLeftSaber(ref saberManager) = left;
			Accessors.SMRightSaber(ref saberManager) = right;
			_saberManagers.ForEach(x => x.Initialize(saberManager));
		}

		private void DidResume()
		{
			_managedSabers.ForEach(x => { if (x != null) { x.gameObject.SetActive(true); } });
		}

		private void DidPause()
		{
			_managedSabers.ForEach(x => { if (x != null) { x.gameObject.SetActive(false); } });
		}

		public void Initialize()
        {
            _safeReady = true;
            while (_temporaryQueue.Count != 0)
            {
                SaberCreated(_temporaryQueue.Dequeue());
            }
            while (_temporaryColorQueue.Count != 0)
            {
                ChangeColor(_temporaryColorQueue.Dequeue());
            }

			_gamePause.didPauseEvent += DidPause;
			_gamePause.didResumeEvent += DidResume;
		}

        /// <summary>
        /// Registers a saber into the effect manager.
        /// </summary>
        /// <param name="saber">The saber being registered.</param>
        public void SaberCreated(Saber saber)
        {
            if (!_safeReady)
            {
                _temporaryQueue.Enqueue(saber);
            }
            else if (saber != null)
            {
                _saberManagers.ForEach(isr => isr.RegisterSaber(saber));
				_managedSabers.Add(saber);
            }
			_siraSaberClashChecker.MultiSaberMode = true;
		}

        /// <summary>
        /// Unregisters a saber from the effect manager.
        /// </summary>
        /// <param name="saber">The saber being unregistered.</param>
        public void SaberDestroyed(Saber saber)
        {
			_managedSabers.Remove(saber);
            _saberManagers.ForEach(isr => isr.UnregisterSaber(saber));
        }

        /// <summary>
        /// Changes the color of a saber in the effect manager. This will update all effects with the correct color.
        /// </summary>
        /// <param name="saber">The saber that's having its color changed.</param>
        public void ChangeColor(Saber saber)
        {
            if (!_safeReady)
            {
                _temporaryColorQueue.Enqueue(saber);
            }
            else if (saber != null)
            {
                _saberManagers.ForEach(isr => isr.ChangeColor(saber));
            }
        }

		public void Dispose()
		{
			_gamePause.didPauseEvent -= DidPause;
			_gamePause.didResumeEvent -= DidResume;
		}
	}
}