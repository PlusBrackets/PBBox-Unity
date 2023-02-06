namespace PBBox
{
    public interface ISortedUpdatable<T> : IUpdatable<T> where T : IUpdateParameter
    {
        int SortedOrder { get; }
    }
}