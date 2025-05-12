/*--------------------------------------------------------
 *Copyright (c) 2016-2023 PlusBrackets
 *@update: 2023.01.11
 *@author: PlusBrackets
 --------------------------------------------------------*/
using System;

namespace PBBox
{
    public interface IReferenceCacheBase
    {
        Type ReferenceType { get; }
        int UsingCount { get; }
        int CachedCount { get; }
        public object Acquire();
        public void Release(object reference);
        public void Remove(int count);
        public void Clear();
    }
}