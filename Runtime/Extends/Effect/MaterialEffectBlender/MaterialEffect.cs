using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PBBox;
//Define USE Odin
#if ODIN_INSPECTOR && UNITY_EDITOR
#define USE_ODIN
#endif
#if USE_ODIN
using Sirenix.OdinInspector;
#endif
//Define End
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace PBBox.Effects
{
    /// <summary>
    /// 材质效果
    /// </summary>
    [System.Serializable]
    public partial class MaterialEffect :ISerializationCallbackReceiver
    {
        [System.Serializable]
        public class Motion
        {
            [Min(0)]
            public float motionDelay = 0;
            public string[] effectShaderNames;
            public Shader[] effectShaders;

            //TODO 之后会仿照SkillConfigObject的Effect，使用Odin序列化和反射配合将不同子类设置合并成一个列表
            [Space]
            public List<FloatParamTransition> floatTransitions;
            [Space]
            public List<ColorParamTransition> colorTransitions;

        }
        [Tooltip("可为空，用于识别effect"), SerializeField]
        private string m_EffectName;
        public string EffectName => m_EffectName;
        [Tooltip("优先度，修改同一个参数时，数字大的效果会覆盖数字小的")]
        public float priority;
        [Space]
        #if USE_ODIN
        [OnValueChanged("UpdateMotionMetaData", true, InvokeOnInitialize = true, InvokeOnUndoRedo = true)]
        #endif
        public Motion[] motions;
        [Space]
        #if USE_ODIN
        [ShowInInspector, EnableIf("@false"), SerializeField]
        #endif
        private float m_TotalDuration;
        public float TotalDuration => m_TotalDuration;
        #if USE_ODIN
        [ShowInInspector, EnableIf("@false"), SerializeField]
        #endif
        private bool m_IsLoopEffect;
        public bool IsLoopEffect => m_IsLoopEffect;
        [Min(0)]
        public float fadeInDuration = 0.15f;
        [Min(0), Tooltip("若大于0，则在stop时渐出，此时stop不会立即清理该效果")]
        public float fadeOutDuration = 0.15f;
        public bool useScaledTime = false;
        public float speedMut = 1.0f;
        // [Tooltip("结束后是否继续保持特效，为true时将不会自动stop")]
        // public bool holdEffect = false;//TODO 未实装

        /// <summary>
        /// 更新Meta data方便游戏中使用
        /// </summary>
        public void UpdateMotionMetaData()
        {
            m_TotalDuration = 0f;
            m_IsLoopEffect = false;
            if (motions == null)
                return;
            foreach (var m in motions)
            {
                CheckMetaDataInTransitions(m.floatTransitions, m, ref m_TotalDuration, ref m_IsLoopEffect);
                CheckMetaDataInTransitions(m.colorTransitions, m, ref m_TotalDuration, ref m_IsLoopEffect);
            }
        }

        private void CheckMetaDataInTransitions(IList list, Motion motion, ref float totalDuration, ref bool isLoopEffect)
        {
            if (list != null)
            {
                foreach (var e in list)
                {
                    var _e = e as BaseParamTransition;
                    float tDur = motion.motionDelay + _e.delay + _e.durtaion;
                    if (totalDuration < tDur)
                    {
                        totalDuration = tDur;
                    }
                    if (_e.loop)
                    {
                        isLoopEffect = true;
                    }
                }
            }
        }

        public void OnBeforeSerialize()
        {
#if !USE_ODIN
            UpdateMotionMetaData();
#endif
        }

        public void OnAfterDeserialize()
        {
        }

    }
}