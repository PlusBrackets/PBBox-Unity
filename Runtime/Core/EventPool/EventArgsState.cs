/*--------------------------------------------------------
 *Copyright (c) 2016-2023 PlusBrackets
 *@update: 2023.01.13
 *@author: PlusBrackets
 --------------------------------------------------------*/
namespace PBBox
{
    /// <summary>
    /// 事件参数的状态
    /// </summary>
    public enum EventArgsState
    {
        Unused = -1,
        Standby,
        Sending,
        Finished,
        Interrupted = 100
    }
}