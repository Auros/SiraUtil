using System;
using UnityEngine;
using Zenject;

namespace SiraUtil.Tools.FPFC
{
    internal class SimpleFPFCSettings : IFPFCSettings, ITickable
    {
        public float FOV => 100;
        public bool Enabled { get; private set; } = true;

        public event Action<IFPFCSettings>? Changed;

        public void Tick()
        {
            if (Input.GetKeyDown(KeyCode.G))
            {
                Enabled = !Enabled;
                Changed?.Invoke(this);
            }
        }
    }
}