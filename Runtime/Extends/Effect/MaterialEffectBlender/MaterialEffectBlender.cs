/*--------------------------------------------------------
 *Copyright (c) 2016-2022 PlusBrackets
 *@update: 2022.11.09
 *@author: PlusBrackets
 --------------------------------------------------------*/
using UnityEngine;
using System;
using System.Collections.Generic;

namespace PBBox.Effects
{
    /// <summary>
    /// 材质效果混合器
    /// </summary>
    public class MaterialEffectBlender
    {
        private struct PlayingEffect : IComparable<PlayingEffect>
        {
            public long uid;
            public float passTime;
            internal float stopTime;
            public MaterialEffect effect;

            public int CompareTo(PlayingEffect other)
            {
                return effect.priority.CompareTo(other.effect.priority);
            }
        }

        public GameObject Owner { get; private set; }
        public MaterialCollection Collection { get; private set; }
        public float timeScale = 1f;

        private Lazy<List<PlayingEffect>> _playingEffects = new Lazy<List<PlayingEffect>>();
        private Lazy<List<long>> _finishedEffects = new Lazy<List<long>>();
        private int _lastPlayingEffectCount = 0;

        /// <summary> 
        /// 是否有任意效果正在执行
        /// </summary>
        /// <value></value>
        public bool IsPlaying => _playingEffects.IsValueCreated && _playingEffects.Value.Count > 0;

        public MaterialEffectBlender(GameObject owner)
        {
            Owner = owner;
            Collection = new MaterialCollection();
        }

        /// <summary>
        /// 播放材质特效
        /// </summary>
        /// <param name="effect"></param>
        /// <returns>该次播放的effect唯一id</returns>
        public long PlayEffect(MaterialEffect effect)
        {
            long uid = PBBox.PBMath.GenSUID();
            _playingEffects.Value.Add(new PlayingEffect
            {
                uid = uid,
                passTime = 0,
                effect = effect,
                stopTime = -1f
            });
            //按照优先度由小到大排序
            _playingEffects.Value.Sort();
            return uid;
        }

        /// <summary>
        /// 停止材质特效
        /// </summary>
        /// <param name="effectID"></param>
        public void StopEffect(long effectID, bool fadeOut = true)
        {
            if (!IsPlaying)
                return;
            int index = _playingEffects.Value.FindIndex(e => e.uid == effectID);
            if (index < 0)
            {
                return;
            }
            if (fadeOut)
            {
                var pe = _playingEffects.Value[index];
                //正在fadeOut，忽略
                if (pe.stopTime >= 0 && pe.passTime - pe.stopTime < pe.effect.fadeOutDuration)
                {
                    return;
                }
                //可以fadeOut，执行fadeOut
                else if (pe.stopTime < 0 && pe.effect.fadeOutDuration > 0)
                {
                    pe.stopTime = pe.passTime;
                    _playingEffects.Value[index] = pe;
                    return;
                }
            }
            //_playingEffects.Value.TryGetValue(uid,out var removedEffect);//TODO 如果需要结束回调
            _playingEffects.Value.RemoveAt(index);
        }

        /// <summary>
        /// 更新Effect
        /// </summary>
        /// <param name="deltaTime"></param>
        public void UpdateEffect(float deltaTime)
        {
            if (!Owner)
                return;
            if (!_playingEffects.IsValueCreated)
                return;
            //lastPlayingEffectCount保证effect全无后也能执行Reset
            if (_playingEffects.Value.Count == 0 && _lastPlayingEffectCount == 0)
                return;
            if (!Collection.Owner)
                Collection.RebuildCollection(Owner);
            //清除上一帧结束的Effect
            if (_finishedEffects.IsValueCreated && _finishedEffects.Value.Count > 0)
            {
                foreach (var uid in _finishedEffects.Value)
                {
                    StopEffect(uid, true);
                }
                _finishedEffects.Value.Clear();
            }
            _lastPlayingEffectCount = 0;
            Collection.ResetAllBlendingBlock();

            //如果清理后effect数为0，pass掉后续的流程
            if (_playingEffects.Value.Count == 0)
                return;

            float scaledDeltaTime = timeScale * deltaTime;
            for (int i = 0; i < _playingEffects.Value.Count; i++)
            {
                var pe = _playingEffects.Value[i];
                long uid = pe.uid;
                float _deltaTime = pe.effect.useScaledTime ? scaledDeltaTime : deltaTime;
                _deltaTime *= pe.effect.speedMut;

                if (IsFinishableEffect(pe))
                {
                    _finishedEffects.Value.Add(uid);
                }

                MixedEffectParamTransition(pe, _deltaTime);

                pe.passTime += _deltaTime;
                _playingEffects.Value[i] = pe;
                _lastPlayingEffectCount++;
            }

            Collection.ApplyAllBlendingBlock();
        }

        private bool IsFinishableEffect(PlayingEffect pe)
        {
            return (pe.stopTime > 0 && pe.passTime - pe.stopTime > pe.effect.fadeOutDuration)
                    || (!pe.effect.IsLoopEffect && pe.passTime >= pe.effect.TotalDuration);
        }

        private void MixedEffectParamTransition(PlayingEffect pe, float deltaTime)
        {
            var effect = pe.effect;
            var passTime = pe.passTime;
            float effective = effect.fadeInDuration > 0 ? Mathf.Clamp01(passTime / effect.fadeInDuration) : 1.0f;
            if (pe.stopTime > 0 && effect.fadeOutDuration > 0)
            {
                effective *= (1f - Mathf.Clamp01((pe.passTime - pe.stopTime) / effect.fadeOutDuration));
            }
            //遍历所有motion，混合参数
            foreach (var m in effect.motions)
            {
                if (m.effectShaderNames != null)
                {
                    foreach (string shaderName in m.effectShaderNames)
                    {
                        _SetToBlendingBlock(shaderName, m);
                    }
                }
                if (m.effectShaders != null)
                {
                    foreach (var shader in m.effectShaders)
                    {
                        _SetToBlendingBlock(shader.name, m);
                    }
                }
            }
            void _SetToBlendingBlock(string shaderName, MaterialEffect.Motion motion)
            {
                Collection.ForeachRendererByShaderName(shaderName, info =>
                {
                    MaterialEffect.BlendPassValues values;
                    values.matInfo = info;
                    values.deltaTime = deltaTime;
                    values.passtime = passTime;
                    values.effective = effective;
                    values.motionDelay = motion.motionDelay;
                    foreach (var t in motion.floatTransitions)
                    {
                        t.BlendTransition(values);
                    }
                    foreach (var t in motion.colorTransitions)
                    {
                        t.BlendTransition(values);
                    }
                });
            }
        }
    }
}