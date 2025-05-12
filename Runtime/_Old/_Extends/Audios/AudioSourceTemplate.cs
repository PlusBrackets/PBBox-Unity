/*--------------------------------------------------------
 *Copyright (c) 2022 PlusBrackets
 *@update: 2022.05.21
 *@author: PlusBrackets
 --------------------------------------------------------*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PBBox
{
    [AddComponentMenu("PBBox/Audio/Audio Source Template")]
    [RequireComponent(typeof(AudioSource))]
    public class AudioSourceTemplate : SimplePoolObject
    {
        [SerializeField]
        AudioSource m_Source = null;
        public AudioSource source
        {
            get
            {
                if (m_Source == null)
                {
                    m_Source = GetComponent<AudioSource>();
                }
                return m_Source;// ?? (m_Source = GetComponent<AudioSource>());
            }
        }

        public string assetName { get; set; } = null;
        public bool isSourceModified { get; set; } = false;
        public bool isRecycleAtStop { get; set; } = true;
        bool m_IsPlayed = false;

#if UNITY_EDITOR
        void OnValidate()
        {
            if (m_Source == null)
            {
                m_Source = GetComponent<AudioSource>();
                delayInGameTime = false;
                lifeTimer.useUnscaleTime = true;
                recycleInDisable = true;
            }
        }
#endif

        protected override void OnSpawned(object data)
        {
            base.OnSpawned(data);
            recycleInDisable = true;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            if (m_Source)
            {
                if (m_Source.isPlaying)
                    m_Source.Stop();
                m_Source.clip = null;
            }
            if (!string.IsNullOrEmpty(assetName))
            {
                AssetManager.ReleaseAsset(assetName);
                assetName = null;
            }
        }

        protected override void LateUpdate()
        {
            base.LateUpdate();
            if (m_Source)
            {
                if (m_IsPlayed)
                {
                    if (!m_Source.isPlaying && isRecycleAtStop)
                    {
                        m_IsPlayed = false;
                        EndLife();
                    }
                }
                else
                {
                    if (m_Source.isPlaying)
                        m_IsPlayed = true;
                }
            }
        }
    }
}