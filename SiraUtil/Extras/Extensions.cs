using SiraUtil.Objects;
using SiraUtil.Submissions;
using Zenject;

namespace SiraUtil.Extras
{
    /// <summary>
    /// Some public extensions.
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Registers a redecorator for the object API.
        /// </summary>
        /// <remarks>
        /// This cannot be called on the App scene, please only call this as you're installing your game related bindings.
        /// </remarks>
        /// <typeparam name="TRegistrator"></typeparam>
        /// <param name="container"></param>
        /// <param name="registrator"></param>
        public static void RegisterRedecorator<TRegistrator>(this DiContainer container, TRegistrator registrator) where TRegistrator : RedecoratorRegistration
        {
            container.AncestorContainers[0].Bind<RedecoratorRegistration>().FromInstance(registrator).AsCached();
        }

        /// <summary>
        /// Is score submission enabled?
        /// </summary>
        /// <param name="levelCompletionResults">The level completion results of the play.</param>
        /// <returns>Whether or not score submission was disabled by SiraUtil.</returns>
        public static bool ScoreSubmissionEnabled(this LevelCompletionResults levelCompletionResults)
        {
            if (levelCompletionResults is SiraLevelCompletionResults siraLevelCompletionResults)
                return siraLevelCompletionResults.ShouldSubmitScores;
            return true;
        }
    }
}