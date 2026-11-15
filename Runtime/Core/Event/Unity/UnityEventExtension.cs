#if UNITY_5_3_OR_NEWER
/*--------------------------------------------------------
 *Copyright (c) 2016-2026 PlusBrackets
 *@update: 2026.11.16
 *@author: PlusBrackets
 --------------------------------------------------------*/
using UnityEngine;
using UnityEngine.Events;
using System;

namespace PBBox
{
    [Serializable]
    public class UEvent_Object : UnityEvent<object> { }
    [Serializable]
    public class UEvent_String : UnityEvent<string> { }
    [Serializable]
    public class UEvent_Int : UnityEvent<int> { }
    [Serializable]
    public class UEvent_Float : UnityEvent<float> { }
    [Serializable]
    public class UEvent_Bool : UnityEvent<bool> { }
}
#endif