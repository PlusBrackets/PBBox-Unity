
using System.Collections.Generic;
using UnityEngine;
using System;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

namespace PBBox.Variables
{
#if ODIN_INSPECTOR
    [DrawWithUnity]
#endif
    [UnityEngine.Scripting.APIUpdating.MovedFrom(true, "PBBox", sourceClassName: "BuffableFloat")]
    public sealed partial class MutableFloat
    {
#if UNITY_EDITOR
        [UnityEditor.CustomPropertyDrawer(typeof(MutableFloat))]
        private class FloatDrawer : Drawer { }
#endif
    }
}