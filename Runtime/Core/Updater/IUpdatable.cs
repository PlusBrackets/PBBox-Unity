namespace PBBox
{
    public interface IUpdatable<T> where T : IUpdateParameter
    {
        IUpdater<T> CurrentUpdater { get; protected internal set; }
        bool IsUpdateEnable { get; }

        void OnUpdate(ref T parameter);
    }

}