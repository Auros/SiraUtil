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

        public bool ShouldInstall(ContextBinding binding)
        {
            return binding.context.GetType() == typeof(T) && binding.context.gameObject.scene.name == _sceneName;
        }
    }
}