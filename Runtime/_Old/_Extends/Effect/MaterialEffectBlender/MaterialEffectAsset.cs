//Define USE Odin
#if ODIN_INSPECTOR && UNITY_EDITOR
#define USE_ODIN
#endif
#if USE_ODIN
using Sirenix.OdinInspector;
#endif
//Define End
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PBBox.Effects
{
    [CreateAssetMenu(menuName = "PBBox/Effects/Material Effect")]
    public class MaterialEffectAsset : ScriptableObject
    {
#if USE_ODIN
        [InlineProperty,HideLabel]
#endif
        public MaterialEffect effect = new MaterialEffect();

    }
}