/*--------------------------------------------------------
 *Copyright (c) 2022 PlusBrackets
 *@update: 2022.05.21
 *@author: PlusBrackets
 --------------------------------------------------------*/
#if (ODIN_INSPECTOR || ODIN_INSPECTOR_3) && UNITY_EDITOR
#define USE_ODIN
#endif
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
#if USE_ODIN
using Sirenix.OdinInspector;
#endif

namespace PBBox.UI
{
    /// <summary>
    /// TODO
    /// 异步显示addressable里的图片
    /// </summary>
    [AddComponentMenu("PBBox/UI/Components/Async Image Shower")]
    public class AsyncImageShower : MonoBehaviour
    {
        protected class Handler
        {
            public Sprite result;
            public string SpriteName { get; private set; }
            public bool IsReleased { get; private set; } = false;

            public Handler(string spriteName)
            {
                this.SpriteName = spriteName;
            }

            public void Release()
            {
                IsReleased = true;
            }
        }

        [SerializeField]
        protected Image m_Image;
        public Image Image => m_Image;
        protected CanvasRenderer m_ImageRenderer;
        [SerializeField, Tooltip("在显示sprite或者解除显示时更改image的gameObject.active状态，若否，则设置CanvasRenderer的alpha")]
        bool m_SetImageActive = true;
        [SerializeField]
        protected string m_SpriteName;
        public bool showOnEnable = false;
        public bool dismissOnDisable = false;
        [Tooltip("设置加载图片的消耗，每帧最大值为100，超过会留到下一帧")]
        public int loadCost = 100;
        public string actionQueueName = ActionQueue.NAME_ResLoad;
        [SerializeField]   
#if USE_ODIN
        [FoldoutGroup("Events")]
#endif     
        UnityEvent m_OnSpriteShow,m_OnSpriteDismiss;
        public UnityEvent onSpriteShow => m_OnSpriteShow ?? (m_OnSpriteShow = new UnityEvent());
        public UnityEvent onSpriteDismiss => m_OnSpriteDismiss ?? (m_OnSpriteDismiss = new UnityEvent());

        Handler m_AsyncHandler;
        public string showingSpriteName => m_AsyncHandler?.SpriteName;
        public bool isShowingSprite => !string.IsNullOrEmpty(m_AsyncHandler?.SpriteName);

        protected virtual void Awake()
        {
            m_ImageRenderer = m_Image.GetComponent<CanvasRenderer>();
            if (!isShowingSprite)
            {
                if (m_SetImageActive)
                {
                    m_Image.gameObject.SetActive(false);
                }
                else
                {
                    m_ImageRenderer.SetAlpha(0);
                }
            }
        }

        protected virtual void OnEnable()
        {
            if (showOnEnable)
            {
                if (m_AsyncHandler == null)
                {
                    Show();
                }
            }
        }

        protected virtual void OnDisable()
        {
            if (dismissOnDisable)
            {
                Dismiss();
            }
        }

        protected virtual void OnDestroy()
        {
            if (m_AsyncHandler == null)
                return;
            m_AsyncHandler.Release();
            if (m_AsyncHandler.result)
            {
                AssetManager.ReleaseAsset(m_AsyncHandler.SpriteName);
            }
        }

        public void Show(string spriteName = null)
        {
            string spName = string.IsNullOrEmpty(spriteName) ? m_SpriteName : spriteName;
            if (spName == showingSpriteName || string.IsNullOrEmpty(spName))
                return;
            Dismiss();
            m_AsyncHandler = new Handler(spName);
            var _handler = m_AsyncHandler;
            ActionQueue.Get(actionQueueName).Do(
                    () => AssetManager.LoadAsset<Sprite>(_handler.SpriteName, sp => OnSpriteLoaded(sp, _handler)),
                    () => _handler != null && !_handler.IsReleased,
                    cost: 100);
        }

        private void OnSpriteLoaded(Sprite sp, Handler handler)
        {
            if (!this || handler.IsReleased || handler != m_AsyncHandler)
            {
                AssetManager.ReleaseAsset(handler.SpriteName);
            }
            else if (sp != null)
            {
                handler.result = sp;
                OnSpriteShow(sp);
                m_OnSpriteShow?.Invoke();
            }
        }

        protected virtual void OnSpriteShow(Sprite sp)
        {
            if (m_Image)
                m_Image.sprite = sp;
            if (m_SetImageActive)
                m_Image.gameObject.SetActive(true);
            else
                m_ImageRenderer.SetAlpha(1);
        }

        public void Dismiss()
        {
            if (m_AsyncHandler == null)
                return;
            if (m_Image)
                m_Image.sprite = null;
            m_AsyncHandler.Release();
            if (m_AsyncHandler.result)
            {
                AssetManager.ReleaseAsset(m_AsyncHandler.SpriteName);
            }
            m_AsyncHandler = null;
            OnSpriteDismiss();
            m_OnSpriteDismiss?.Invoke();
        }

        protected virtual void OnSpriteDismiss()
        {
            if (m_SetImageActive)
                m_Image.gameObject.SetActive(false);
            else
                m_ImageRenderer.SetAlpha(0);
        }
    }
}