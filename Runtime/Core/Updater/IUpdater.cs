namespace PBBox
{
    public interface IUpdater<T> where T : IUpdateParameter
    {
        bool IsUpdating { get; }

        void Attach(IUpdatable<T> updatable);

        void Unattach(IUpdatable<T> updatable, bool immediately = false);

        void Update(ref T param);
    }
}