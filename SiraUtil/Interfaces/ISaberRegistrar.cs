namespace SiraUtil.Interfaces
{
    public interface ISaberRegistrar
    {
		void Initialize(SaberManager saberManager);
        void ChangeColor(Saber saber);
        void RegisterSaber(Saber saber);
        void UnregisterSaber(Saber saber);
    }
}