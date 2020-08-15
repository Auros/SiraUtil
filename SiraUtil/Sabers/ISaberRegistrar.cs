namespace SiraUtil.Sabers
{
    public interface ISaberRegistrar
    {
        void ChangeColor(Saber saber);
        void RegisterSaber(Saber saber);
        void UnregisterSaber(Saber saber);
    }
}