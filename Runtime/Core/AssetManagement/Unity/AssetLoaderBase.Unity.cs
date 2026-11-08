#if UNITY_5_3_OR_NEWER
/*--------------------------------------------------------
 *Copyright (c) 2016-2025 PlusBrackets
 *@update: 2025.06.03
 *@author: PlusBrackets
 --------------------------------------------------------*/
using System.Collections.Generic;
using System.Collections;
using UnityEngine;

namespace PBBox.AssetManagement
{
    public abstract partial class AssetLoaderBase
    {
        protected bool IsAssetVaildUnity()
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
#endif