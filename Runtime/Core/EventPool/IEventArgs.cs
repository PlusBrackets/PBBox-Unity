/*--------------------------------------------------------
 *Copyright (c) 2016-2023 PlusBrackets
 *@update: 2023.01.13
 *@author: PlusBrackets
 --------------------------------------------------------*/
namespace PBBox
{
    /// <summary>
    /// 事件参数拓展接口
    /// </summary>
    public interface IEventArgs
    {
        EventArgsState EventState { get; protected internal set; }
        void Release();
    }
}