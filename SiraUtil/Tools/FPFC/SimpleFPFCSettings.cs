using System;
using UnityEngine;
using Zenject;

namespace SiraUtil.Tools.FPFC
{
    internal class SimpleFPFCSettings : IFPFCSettings, ITickable
    {
        public bool Enabled { get; private set; } = true;

        public event Action<bool>? Changed;

        public void Tick()
        {
            if (Input.GetKeyDown(KeyCode.G))
            {
                Enabled = !Enabled;
                Changed?.Invoke(Enabled);
            }
        }
    }
}