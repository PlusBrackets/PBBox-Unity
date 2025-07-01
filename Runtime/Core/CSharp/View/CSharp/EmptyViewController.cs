using PBBox.View;

namespace PBBox.UI
{
    /// <summary>
    /// 空视图控制器类，继承自ControllerBase。
    /// 该类用于创建一个没有具体实现的视图控制器
    /// </summary>
    public sealed class EmptyViewController : ControllerBase
    {
        public EmptyViewController(string id, IViewFactory viewLoader) : base(id, viewLoader)
        {
        }
    }
}