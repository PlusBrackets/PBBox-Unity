using System;
/*--------------------------------------------------------
 *Copyright (c) 2022 PlusBrackets
 *@update: 2022.05.12
 *@author: PlusBrackets
 --------------------------------------------------------*/
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using PBBox;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;

namespace PBBox
{
    /// <summary>
    /// SceneLoader,目前仅供单线scene读取切换
    /// </summary>
    [AddComponentMenu("")]
    public class SceneLoader : SingleBehaviour<SceneLoader>
    {
        protected override bool hideInHierarchy => true;
        /// <summary>
        /// 加载场景的名称
        /// </summary>
        /// <value></value>
        public string loadingSceneName { get; set; } = "GameLoadingScene";
        public BindableValue<float> bindableLoadingProgress { get; private set; } = new BindableValue<float>(1f);
        public float loadingProgress { get => bindableLoadingProgress.Value; private set => bindableLoadingProgress.Value = value; }

        // List<AsyncOperationHandle<SceneInstance>> m_LoadedAddressableScenes = new List<AsyncOperationHandle<SceneInstance>>();
        /// <summary>
        /// 是否正在读取场景
        /// </summary>
        /// <value></value>
        public bool isLoadingScene { get; private set; } = false;
        bool isLoadingLocalScene
        {
            get
            {
                return m_LoadOpera != null;
            }
        }
        AsyncOperation m_LoadOpera;
        AsyncOperationHandle<SceneInstance> m_AddressableLoadOpera;
        bool m_IsAutoActiveAfterLoaded = true;

        /// <summary>
        /// 当场景加载开始时调用
        /// </summary>
        public event Action<string> onLoadStarted;
        /// <summary>
        /// 当场景加载完成时调用
        /// </summary>
        public event Action<string> onLoadFinish;
        /// <summary>
        /// 当被加载的场景激活完成后调用
        /// </summary>
        public event Action<string> onLoadedSceneActived;

        /// <summary>
        /// 单线切换场景
        /// </summary>
        /// <param name="sceneName"></param>
        /// <param name="isAutoActive">是否自动激活场景,若否，则需要调用SetAllowActiveScene来激活已加载的场景</param>
        /// <param name="isEnterLoadingScene">是否进入加载场景</param>
        public static void SwitchScene(string sceneName, bool isAutoActive = true, bool isEnterLoadingScene = true)
            => Instance.SwitchSceneInternal(sceneName, isAutoActive, isEnterLoadingScene);

        /// <summary>
        /// 设置自动激活场景
        /// </summary>
        /// <param name="allow"></param>
        public static void SetAllowActiveScene(bool allow)
        {
            Instance.m_IsAutoActiveAfterLoaded = allow;
        }

        void SwitchSceneInternal(string sceneName, bool isAutoActive, bool isEnterLoadingScene)
        {
            if (isLoadingScene)
            {
                DebugUtils.LogError("正在加载场景流程中，请等待加载完毕");
                return;
            }
            loadingProgress = 0f;
            isLoadingScene = true;
            isEnterLoadingScene = isEnterLoadingScene && String.IsNullOrEmpty(loadingSceneName);
            AsyncOperation op = null;
            AsyncOperationHandle<SceneInstance> aop = default;
            if (isEnterLoadingScene)
            {
                if (Application.CanStreamedLevelBeLoaded(loadingSceneName))
                {
                    op = SceneManager.LoadSceneAsync(loadingSceneName);
                    op.allowSceneActivation = false;
                }
                else
                {
                    aop = Addressables.LoadSceneAsync(loadingSceneName, LoadSceneMode.Single, false);
                }
            }
            System.Action PrepearLoadScene = async () =>
            {
                if (op != null)
                {
                    op.allowSceneActivation = true;
                    while (!op.isDone)
                    {
                        await Task.Delay(1);
                    }
                }
                else if (aop.IsValid())
                {
                    await aop.Task;
                    var op = aop.Result.ActivateAsync();
                    while (!op.isDone)
                    {
                        await Task.Delay(1);
                    }

                }
                StartCoroutine(StartLoadScene(sceneName));
            };
            PrepearLoadScene();
        }

        IEnumerator StartLoadScene(string sceneName)
        {
            yield return null;
            onLoadStarted?.Invoke(sceneName);
            if (Application.CanStreamedLevelBeLoaded(sceneName))
            {
                m_LoadOpera = SceneManager.LoadSceneAsync(sceneName);
                m_LoadOpera.allowSceneActivation = false;
                while (m_LoadOpera.progress < 0.9f)
                {
                    yield return null;
                    loadingProgress = m_LoadOpera.progress;
                }
                onLoadFinish?.Invoke(sceneName);
                while (!m_LoadOpera.isDone)
                {
                    m_LoadOpera.allowSceneActivation = m_IsAutoActiveAfterLoaded;
                    yield return null;
                    loadingProgress = m_LoadOpera.progress;
                }
            }
            else
            {
                m_AddressableLoadOpera = Addressables.LoadSceneAsync(key: sceneName, activateOnLoad: false);
                while (!m_AddressableLoadOpera.IsDone)
                {
                    yield return null;
                    loadingProgress = Mathf.Clamp(m_AddressableLoadOpera.PercentComplete, 0f, 0.9f);
                }
                onLoadFinish?.Invoke(sceneName);
                AsyncOperation tempOpera = m_AddressableLoadOpera.Result.ActivateAsync();
                while (!tempOpera.isDone)
                {
                    tempOpera.allowSceneActivation = m_IsAutoActiveAfterLoaded;
                    yield return null;
                    loadingProgress = Mathf.Clamp(tempOpera.progress, 0.9f, 1f);
                }
            }
            m_LoadOpera = null;
            m_AddressableLoadOpera = default;
            isLoadingScene = false;
            loadingProgress = 1f;
            onLoadedSceneActived?.Invoke(sceneName);
        }

    }
}