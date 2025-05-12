/*--------------------------------------------------------
 *Copyright (c) 2016-2022 PlusBrackets
 *@update: 2022.12.12
 *@author: PlusBrackets
 --------------------------------------------------------*/
namespace PBBox
{
    /// <summary>
    /// 单例生命周期，单例创建或销毁时会触发调用对应函数
    /// </summary>
    public interface ISingletonLifecycle
    {
        void OnCreateAsSingleton();
        void OnDestroyAsSingleton();
    }
}