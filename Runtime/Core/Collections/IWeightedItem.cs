/*--------------------------------------------------------
 *Copyright (c) 2022 PlusBrackets
 *@update: 2022.03.29
 *@author: PlusBrackets
 --------------------------------------------------------*/
using System.Collections.Generic;

namespace PBBox.Collections
{
    /// <summary>
    /// 加权项
    /// </summary>
    public interface IWeightedItem
    {
        float Weights { get; set; }
    }

    public interface IWeightedItem<TContent> : IWeightedItem
    {
        TContent Content { get; }
    }
}