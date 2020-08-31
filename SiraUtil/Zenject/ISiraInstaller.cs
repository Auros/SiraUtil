using Zenject;
using UnityEngine;

namespace SiraUtil.Zenject
{
    /// <summary>
    /// The interface which handles custom installations into SiraUtil's Zenject system.
    /// </summary>
    public interface ISiraInstaller
    {
        void Install(DiContainer container, GameObject source);
    }
}