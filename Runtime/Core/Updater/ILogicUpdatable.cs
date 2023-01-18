namespace PBBox
{
    public interface ILogicUpdatable{
        bool IsVaild { get; protected internal set; }
    }

    public interface ILogicUpdatable<T> : ILogicUpdatable where T : ILogicUpdateParameter
    {
        int UpdateOrder { get; }
        bool IsEnable { get; }

        void OnUpdate(T parameter);
    }
}