/*--------------------------------------------------------
 *Copyright (c) 2016-2022 PlusBrackets
 *@update: 2022.11.09
 *@author: PlusBrackets
 --------------------------------------------------------*/
using UnityEngine;
//Define USE Odin
#if ODIN_INSPECTOR && UNITY_EDITOR
#define USE_ODIN
#endif
#if USE_ODIN
using Sirenix.OdinInspector;
#endif
//Define End

namespace PBBox.Effects
{
    //内部类
    public partial class MaterialEffect
    {
        private const float F_ZERO = 1E-6F;

        public enum BlendMode
        {
            Replace,
            Multiply,
            Add
        }

        [System.Flags]
        public enum ChannelMask
        {
            All = R | G | B | A,
            R = 1,
            G = 1 << 1,
            B = 1 << 2,
            A = 1 << 3,
        }

        public struct BlendPassValues
        {
            public MaterialCollection.RendererMatInfo matInfo;
            public float passtime;
            public float deltaTime;
            public float effective;
            public float motionDelay;
        }

        public abstract class BaseParamTransition
        {
            #if USE_ODIN
            [Required]
            #endif
            public string paramName;
            [Range(0, 1)]
            public float effective = 1f;
            [Min(0)]
            public float durtaion = 1f;
            [Min(0)]
            public float delay = 0f;
            public bool loop = false;
            [Tooltip("权重，有效范围为[0,1]")]
            public AnimationCurve weight = AnimationCurve.Linear(0, 1, 1, 1);
            [Space]
            public BlendMode blendMode;

            internal void BlendTransition(BlendPassValues values)
            {
                float _delay = delay + values.motionDelay;
                //Do any internal handle;
                if (values.passtime < _delay)
                    return;
                float elapsedTime = values.passtime - _delay;
                float progress = GetProgress(elapsedTime);
                values.effective *= effective * Mathf.Clamp01(weight.Evaluate(progress));

                OnBlendTransition(values, elapsedTime, progress);
            }

            protected virtual float GetProgress(float elapsedTime)
            {
                return durtaion > 0 ? elapsedTime / durtaion : 0;
            }

            protected abstract void OnBlendTransition(BlendPassValues values, float elapsedTime, float progress);

            protected static float GetBlendedValue(BlendMode blendMode, float value, float toValue, float weight)
            {
                switch (blendMode)
                {
                    case BlendMode.Replace:
                        toValue = Mathf.Lerp(value, toValue, weight);
                        break;
                    case BlendMode.Multiply:
                        toValue = value * Mathf.Lerp(1f, toValue, weight);
                        break;
                    case BlendMode.Add:
                        toValue = value + Mathf.Lerp(0, toValue, weight);
                        break;
                }
                return toValue;
            }

            protected static Color GetBlendedValue(BlendMode blendMode, Color value, Color toValue, float weight)
            {
                switch (blendMode)
                {
                    case BlendMode.Replace:
                        toValue = Color.Lerp(value, toValue, weight);
                        break;
                    case BlendMode.Multiply:
                        toValue = value * Color.Lerp(Color.white, toValue, weight);
                        break;
                    case BlendMode.Add:
                        toValue = value + Color.Lerp(Color.clear, toValue, weight);
                        break;
                }
                return toValue;
            }
        }

        /// <summary>
        /// Float值变化
        /// </summary>
        [System.Serializable]
        public class FloatParamTransition : BaseParamTransition
        {
            public Vector2 curveMinMax = Vector2.up;
            [Tooltip("变化曲线，有效范围为[0,1]，会remap成curveMinMax")]
            public AnimationCurve curve = AnimationCurve.EaseInOut(0, 0, 1, 1);

            protected override void OnBlendTransition(BlendPassValues values, float elapsedTime, float progress)
            {
                if (values.effective < F_ZERO)
                    return;
                float toValue = curve.Evaluate(progress);
                float value = values.matInfo.GetFloat(paramName);
                toValue = PBMath.RemapClamp(toValue, 0, 1, curveMinMax.x, curveMinMax.y);
                toValue = GetBlendedValue(blendMode, value, toValue, values.effective);
                values.matInfo.SetFloat(paramName, toValue);
            }
        }

        /// <summary>
        /// 颜色变化，只控制rgb，a为影响力（权重）
        /// </summary>
        [System.Serializable]
        public class ColorParamTransition : BaseParamTransition
        {
            [GradientUsage(true)]
            [Tooltip("颜色变化，只控制rgb，a为影响力（权重）")]
            public Gradient color;
            public ChannelMask chanel = ChannelMask.R | ChannelMask.G | ChannelMask.B;

            protected override void OnBlendTransition(BlendPassValues values, float elapsedTime, float progress)
            {
                if (values.effective < F_ZERO)
                    return;
                Color toCol = color.Evaluate(loop? progress - (int)progress:progress);
                Color col = values.matInfo.GetColor(paramName);
                toCol = GetBlendedValue(blendMode, col, toCol, values.effective);
                toCol.r = chanel.HasFlag(ChannelMask.R) ? toCol.r : col.r;
                toCol.g = chanel.HasFlag(ChannelMask.G) ? toCol.g : col.g;
                toCol.b = chanel.HasFlag(ChannelMask.B) ? toCol.b : col.b;
                toCol.a = chanel.HasFlag(ChannelMask.A) ? toCol.a : col.a;
                values.matInfo.SetColor(paramName, toCol);
            }
        }

    }
}