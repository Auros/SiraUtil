namespace SiraUtil.Interfaces
{
    /// <summary>
    /// An interface used to modify in game prefabs. The prefab type must be supported in order for it to have any actual effect.
    /// </summary>
    /// <typeparam name="T">The type of the original prefab.</typeparam>
    public interface IPrefabProvider<T>
    {
        /// <summary>
        /// Modify the prefab here. Return the prefab, or your own if you want to fullly replace it.
        /// </summary>
        /// <param name="original">The original prefab.</param>
        /// <returns>The modified prefab.</returns>
        T Modify(T original);

        /// <summary>
        /// Chaining a provider will allow multiple providers to affect a single prefab. They will go in order by priority dictated by the <seealso cref="IModelProvider"/>, and any non-chain providers will be skipped. If the highest priority provider is non-chainable then nothing will be chained to it.
        /// </summary>
        bool Chain { get; }
    }
}