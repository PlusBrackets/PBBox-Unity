/*--------------------------------------------------------
 *Copyright (c) 2016-2025 PlusBrackets
 *@update: 2025.06.03
 *@author: PlusBrackets
 --------------------------------------------------------*/
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
    }
}