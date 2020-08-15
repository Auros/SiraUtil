using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace SiraUtil.Sabers
{
    public class SiraSaberEffectManager : MonoBehaviour
    {
        private bool _safeReady = false;
        private readonly Queue<Saber> _temporaryQueue = new Queue<Saber>();
        private readonly Queue<Saber> _temporaryColorQueue = new Queue<Saber>();
        private readonly List<ISaberRegistrar> _saberManagers = new List<ISaberRegistrar>();

        public IEnumerator Start()
        {
            // Unfortunately, this is pretty WeirdChamp, I'm sorry.
            yield return new WaitUntil(() => Resources.FindObjectsOfTypeAll<SiraSaberBurnMarkArea>().Any());
            _saberManagers.Add(Resources.FindObjectsOfTypeAll<SiraSaberBurnMarkArea>().First());
            _saberManagers.Add(Resources.FindObjectsOfTypeAll<SiraSaberClashChecker>().First());
            _saberManagers.Add(Resources.FindObjectsOfTypeAll<SiraSaberBurnMarkSparkles>().First());
            _saberManagers.Add(Resources.FindObjectsOfTypeAll<SiraObstacleSaberSparkleEffectManager>().First());
            yield return new WaitForSecondsRealtime(0.1f); // Wait for any initial created sabers, sometimes their color isn't set immediately.
            _safeReady = true;
            while (_temporaryQueue.Count != 0)
            {
                SaberCreated(_temporaryQueue.Dequeue());
            }
            while (_temporaryColorQueue.Count != 0)
            {
                ChangeColor(_temporaryColorQueue.Dequeue());
            }
        }

        public void SaberCreated(Saber saber)
        {
            if (!_safeReady)
            {
                _temporaryQueue.Enqueue(saber);
            }
            else if (saber != null)
            {
                _saberManagers.ForEach(isr => isr.RegisterSaber(saber));
            }
        }

        public void SaberDestroyed(Saber saber)
        {
            _saberManagers.ForEach(isr => isr.UnregisterSaber(saber));
        }

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
    }
}