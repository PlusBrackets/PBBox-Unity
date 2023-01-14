/*--------------------------------------------------------
 *Copyright (c) 2016-2023 PlusBrackets
 *@update: 2023.01.11
 *@author: PlusBrackets
 --------------------------------------------------------*/
namespace PBBox
{
    public interface IReferenceCache : IReferenceCacheBase
    {
        public T Acquire<T>() where T : class, new();
        public void PreCreate(int count);
        public void PreCreate<T>(int count) where T : class, new();
    }
}