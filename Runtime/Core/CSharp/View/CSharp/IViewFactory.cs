/*--------------------------------------------------------
 *Copyright (c) 2016-2025 PlusBrackets
 *@update: 2025.06.19
 *@author: PlusBrackets
 --------------------------------------------------------*/
using System.Threading.Tasks;

namespace PBBox.View
{
    /// <summary>
    /// 视图工厂接口
    /// <para>用于创建和释放视图实例</para>
    /// </summary>
    public interface IViewFactory
    {
        IView CreateView(IController controller);
        Task<IView> CreateViewAsync(IController controller);
        void ReleaseView(IController controller);
    }
}
