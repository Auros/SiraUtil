namespace SiraUtil.Interfaces
{
    public interface IPrefabProvider<T>
    {
        T Modify(T original);
        bool Chain { get; }
    }
}