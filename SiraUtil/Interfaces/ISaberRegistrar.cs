namespace SiraUtil.Interfaces
{
    /// <summary>
    /// An interface for a manager to handle a dynamic number of sabers.
    /// </summary>
    public interface ISaberRegistrar
    {
        void Initialize(SaberManager saberManager);
        void ChangeColor(Saber saber);
        void RegisterSaber(Saber saber);
        void UnregisterSaber(Saber saber);
    }
}