using System;
using System.Collections.Generic;
using Zenject;

namespace SiraUtil.Zenject.Internal.Filters
{
    internal class ContextedNamedSceneInstallFilter<T> : IInstallFilter where T : Context
    {
        private readonly string _sceneName;

        public ContextedNamedSceneInstallFilter(string sceneName)
        {
            _sceneName = sceneName;
        }

        public bool ShouldInstall(Context context, IEnumerable<Type> installerBindings)
        {
            return context.GetType() == typeof(T) && context.gameObject.scene.name == _sceneName;
        }
    }
}