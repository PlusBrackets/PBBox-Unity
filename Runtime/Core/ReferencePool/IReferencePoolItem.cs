/*--------------------------------------------------------
 *Copyright (c) 2016-2023 PlusBrackets
 *@update: 2023.01.04
 *@author: PlusBrackets
 --------------------------------------------------------*/
namespace PBBox
{
    /// <summary>
    /// 引用类型接口，可接收引用池调用信息
    /// </summary>
    public interface IReferencePoolItem
    {
        bool IsUsing { get; protected internal set; }
        void OnReferenceAcquire();
        void OnReferenceRelease();
    }
}