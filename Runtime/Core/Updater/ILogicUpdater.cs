/*--------------------------------------------------------
 *Copyright (c) 2016-2023 PlusBrackets
 *@update: 2023.02.16
 *@author: PlusBrackets
 --------------------------------------------------------*/
namespace PBBox
{
    /// <summary>
    /// 更新器
    /// </summary>
    /// <typeparam name="TUpdater"></typeparam>
    public interface ILogicUpdater<TUpdater> where TUpdater : ILogicUpdater<TUpdater>
    {
        bool IsUpdating { get; }
        void UpdateHandlers(float deltaTime);
        void Attach(ILogicUpdateHandler<TUpdater> handler);
        void Unattach(ILogicUpdateHandler<TUpdater> handler, bool immediately = false);
    }

}