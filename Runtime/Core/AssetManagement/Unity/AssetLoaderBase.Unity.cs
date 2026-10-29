/*--------------------------------------------------------
 *Copyright (c) 2016-2025 PlusBrackets
 *@update: 2025.06.03
 *@author: PlusBrackets
 --------------------------------------------------------*/
using System.Collections.Generic;
using System.Collections;
using UnityEngine;

namespace PBBox
{
    public abstract partial class AssetLoaderBase
    {
        protected bool IsVaildUnity()
        {
            if (m_Asset is Object unityObject)
            {
                return unityObject != null;
            }
            return true;
        }

        protected IList<TObject> ConvertToList<TObject>(IEnumerable array)
        {
            if (array == null)
            {
                return new List<TObject>();
            }
            var list = new List<TObject>();
            foreach (var item in array)
            {
                if (item is TObject asset)
                {
                    list.Add(asset);
                }
            }
            return list;
        }
    }
}