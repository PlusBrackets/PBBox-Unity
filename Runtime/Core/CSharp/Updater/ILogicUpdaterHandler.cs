/*--------------------------------------------------------
 *Copyright (c) 2016-2023 PlusBrackets
 *@update: 2023.02.16
 *@author: PlusBrackets
 --------------------------------------------------------*/
namespace PBBox
{
    /// <summary>
    /// 更新处理，可给定排列序号
    /// </summary>
    /// <typeparam name="TUpdater"></typeparam>
    public interface ILogicUpdateHandler<TUpdater> where TUpdater : ILogicUpdater<TUpdater>
    {
        TUpdater CurrentUpdater { get; protected internal set; }
        int SortedOrder { get; }
        void OnUpdate(float deltaTime);
    }
}