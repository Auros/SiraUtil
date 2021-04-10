namespace SiraUtil.Zenject.Internal.Filters
{
    internal interface IInstallFilter
    {
        bool ShouldInstall(ContextBinding binding);
    }
}   