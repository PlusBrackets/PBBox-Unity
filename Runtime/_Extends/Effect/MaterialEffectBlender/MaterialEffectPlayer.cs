using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PBBox.Effects
{
    /// <summary>
    /// 材质效果混合播放器
    /// </summary>
    [AddComponentMenu("PBBox/Effects/Material Effect Player")]
    public class MaterialEffectPlayer : MonoBehaviour
    {
        private MaterialEffectBlender _blender;
        public MaterialEffectBlender Blender => _blender;
        public GameObject Owner => _blender?.Owner;

        private void Awake()
        {
            _blender = new MaterialEffectBlender(gameObject);
        }

        private void LateUpdate()
        {
            _blender.UpdateEffect(Time.deltaTime);
        }

        public static long PlayEffect(GameObject owner, MaterialEffect effect)
        {
            var blender = owner.GetComponent<MaterialEffectPlayer>();
            if (!blender)
            {
                blender = owner.AddComponent<MaterialEffectPlayer>();
            }
            return blender.Blender.PlayEffect(effect);
        }

        public static void StopEffect(GameObject owner, long effectId)
        {
            var blender = owner.GetComponent<MaterialEffectPlayer>();
            if (!blender)
                return;
            blender.Blender.StopEffect(effectId);
        }
    }


}