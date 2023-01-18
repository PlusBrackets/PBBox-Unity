namespace PBBox
{
    public interface ILogicUpdater<T> where T : ILogicUpdateParameter
    {
        void Add(ILogicUpdatable<T> updatable);

        void Remove(ILogicUpdatable<T> updatable);
    }
}