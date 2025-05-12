using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PBBox
{
    /*
    TODO 增加PlaySFX，PlayBGM，FadeAudio等功能，绑定AudioMixer，分类为BGM，SFX和其他自定义分类，使用scriptableObject 进行设置或使用代码设置
    */
    /// <summary>
    /// 音频管理器
    /// </summary>
    [AddComponentMenu("")]
    public class AudiosManager : SingleBehaviour<AudiosManager>
    {
        const string DEFAULT_SOURCE_TEMPLATE_NAME = "AudioSource_Default";
        Lazy<Dictionary<string, AudioSource>> __SourceTemplates;
        Dictionary<string, AudioSource> m_SourceTemplates => __SourceTemplates.Value;
        AudioSource m_DefaultSourceTemplate;
        public AudioSource defaultSourceTemplate
        {
            get
            {
                if (m_DefaultSourceTemplate == null)
                {
                    if (!m_SourceTemplates.TryGetValue(DEFAULT_SOURCE_TEMPLATE_NAME, out m_DefaultSourceTemplate))
                    {
                        var obj = new GameObject("_DefaultSourceTemplate");
                        obj.transform.SetParent(transform);
                        m_DefaultSourceTemplate = obj.AddComponent<AudioSource>();
                        obj.AddComponent<AudioSourceTemplate>();
                        obj.SetActive(false);
                        m_SourceTemplates[DEFAULT_SOURCE_TEMPLATE_NAME] = m_DefaultSourceTemplate;
                    }
                }
                return m_DefaultSourceTemplate;
            }
            set
            {
                SetTemplate(DEFAULT_SOURCE_TEMPLATE_NAME, value);
            }
        }
        /// <summary>
        /// 有哪些Template是从AssetManager中加载的
        /// </summary>
        Lazy<HashSet<string>> m_TemplatesFromAssetManager;

        protected override void InitAsInstance()
        {
            base.InitAsInstance();
            __SourceTemplates = new Lazy<Dictionary<string, AudioSource>>();
            m_TemplatesFromAssetManager = new Lazy<HashSet<string>>();
        }

        public AudioSource GetTemplate(string name, bool returnIfNotExist = false, bool fromAssetManager = false)
        {
            if (!m_SourceTemplates.TryGetValue(name, out var template))
            {
                if (fromAssetManager)
                {
                    template = AssetManager.LoadAssetSync<GameObject>(name)?.GetComponent<AudioSource>();
                    if (template != null)
                    {
                        m_TemplatesFromAssetManager.Value.Add(name);
                        template.GetOrAddComponent<AudioSourceTemplate>();
                        m_SourceTemplates[name] = template;
                    }
                }
                if (template == null && returnIfNotExist)
                {
                    return defaultSourceTemplate;
                }
            }
            return template;
        }

        public void SetTemplate(string name, AudioSource template)
        {
            if (m_TemplatesFromAssetManager.Value.Contains(name))
            {
                AssetManager.ReleaseAsset(name);
                m_TemplatesFromAssetManager.Value.Remove(name);
            }
            m_SourceTemplates[name] = template;
            if (name == DEFAULT_SOURCE_TEMPLATE_NAME)
            {
                m_DefaultSourceTemplate = template;
            }
        }

        //TODO
        void ResetPropertiesFromSource(AudioSource src, AudioSource dst)
        {
            dst.playOnAwake = false;
            dst.loop = src.loop;
            dst.ignoreListenerVolume = src.ignoreListenerVolume;
            dst.ignoreListenerPause = src.ignoreListenerPause;
            dst.velocityUpdateMode = src.velocityUpdateMode;
            dst.panStereo = src.panStereo;
            dst.spatialBlend = src.spatialBlend;
            dst.spatialize = src.spatialize;
            dst.spatializePostEffects = src.spatializePostEffects;
            dst.reverbZoneMix = src.reverbZoneMix;
            dst.bypassEffects = src.bypassEffects;
            dst.bypassListenerEffects = src.bypassListenerEffects;
            dst.bypassReverbZones = src.bypassReverbZones;
            dst.dopplerLevel = src.dopplerLevel;
            dst.spread = src.spread;
            dst.priority = src.priority;
            dst.mute = src.mute;
            dst.minDistance = src.minDistance;
            dst.maxDistance = src.maxDistance;
            dst.pitch = src.pitch;
            dst.volume = src.volume;
        }

        //TODO 要支持直接传入AudioClip，从AssetLoad过来的需要在适当的时机释放
        public AudioSource SpawnAudio(string audioName, float? volume = null, Vector3? position = null, Transform parent = null,
                 string sourceTemplate = DEFAULT_SOURCE_TEMPLATE_NAME, bool? loop = null, float? fixedDuration = null, bool autoPlay = true, bool recycleAtStop = true)
        {
            Vector3 pos = position.HasValue ? position.Value : Vector3.zero;
            Transform p = parent ?? transform;
            AudioSource template = GetTemplate(sourceTemplate, true, true);
            float vol = volume.HasValue ? volume.Value : template.volume;

            AudioSourceTemplate apo = PoolsManager.GetPool("Audios", template.gameObject, transform).Spawn(new SimplePool.SpawnParam()
            {
                position = pos,
                parent = p
            })?.GetComponent<AudioSourceTemplate>();
            // if (apo.isSourceModified)
            // {
            //     ResetPropertiesFromSource(template, apo.source);
            // }
            ResetPropertiesFromSource(template, apo.source);
            var _source = apo.source;
            _source.playOnAwake = false;
            _source.volume = vol;
            if (loop.HasValue)
                _source.loop = loop.Value;
            if (!string.IsNullOrEmpty(audioName))
            {
                _source.clip = AssetManager.LoadAssetSync<AudioClip>(audioName);
            }
            float _dur = fixedDuration.HasValue ? fixedDuration.Value : -1;
            apo.lifeTime = _dur;
            apo.isRecycleAtStop = recycleAtStop;
            if (autoPlay && _source.clip)
            {
                _source.Play();
            }
            return _source;
        }

        /// <summary>
        /// 将从0~1的volume值转成分贝（仅近似模拟，非正确转换）
        /// </summary>
        /// <param name="volume"></param>
        /// <returns></returns>
        public static float ParseVolumeToDB(float volume)
        {
            return PBMath.RemapClamp(Mathf.Pow(volume, 0.2f), 0f, 1f, -80f, 0f);
        }

        /// <summary>
        /// 将-80~0的dB近似转为0f~1f的volume值
        /// </summary>
        /// <param name="dB"></param>
        /// <returns></returns>
        public static float ParseDBToVolume(float dB)
        {
            return Mathf.Pow(PBMath.RemapClamp(dB, -80f, 0f, 0f, 1f), 5f);
        }
    }


    public static partial class PBExtensions
    {
        [System.Obsolete]
        public static void MarkModified(this AudioSource source, bool flag)
        {
            var apo = source.GetComponent<AudioSourceTemplate>();
            if (apo == null)
                return;
            apo.isSourceModified = flag;
        }
    }
}