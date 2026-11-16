using System;
using System.Collections.Generic;
using Zenject;

namespace SiraUtil.Zenject.Internal.Filters
{
    internal class ContextedNamedSceneInstallFilter<T> : ContextInstallFilter<T> where T : Context
    {
        private readonly string _sceneName;

        public ContextedNamedSceneInstallFilter(string sceneName)
        {
            _sceneName = sceneName;
        }

        public override bool ShouldInstall(Context context, IEnumerable<Type> installerBindings)
        {
            return base.ShouldInstall(context, installerBindings) && context.gameObject.scene.name == _sceneName;
        }
    }
}