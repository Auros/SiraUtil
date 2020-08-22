using Zenject;
using UnityEngine;

namespace SiraUtil.Zenject
{
    public interface ISiraInstaller
    {
        void Install(DiContainer container, GameObject source);
    }
}