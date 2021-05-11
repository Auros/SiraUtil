namespace SiraUtil.Affinity
{
    internal interface IAffinityPatcher
    {
        void Patch(IAffinity affinity);
        void Unpatch(IAffinity affinity);
    }
}