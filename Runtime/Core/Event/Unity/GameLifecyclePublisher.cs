#if UNITY_5_3_OR_NEWER
/*--------------------------------------------------------
 *Copyright (c) 2016-2026 PlusBrackets
 *@update: 2026.11.13
 *@author: PlusBrackets
 --------------------------------------------------------*/

using UnityEngine;

namespace PBBox.Event
{
    [DisallowMultipleComponent, AddComponentMenu("")]
    internal class UnityLifecyclePublisher : MonoBehaviour
    {
        private static UnityLifecyclePublisher _instance;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Initialize()
        {
            if (_instance == null)
            {
                GameObject go = new GameObject("GameLifecyclePublisher");
                go.hideFlags = HideFlags.HideAndDontSave;
                _instance = go.AddComponent<UnityLifecyclePublisher>();
                DontDestroyOnLoad(go);
            }
        }

        private void OnDestroy()
        {
            _instance = null;
        }

        private void Update()
        {
            PBEvents.FrameUpdateSubject.Publish(new UpdateFrameEvent(Time.deltaTime, Time.unscaledDeltaTime));
        }

        private void LateUpdate()
        {
            PBEvents.LateUpdateSubject.Publish(new UpdateFrameEvent(Time.deltaTime, Time.unscaledDeltaTime));
        }

        private void FixedUpdate()
        {
            PBEvents.FixedUpdateSubject.Publish(new UpdateFrameEvent(Time.fixedDeltaTime, Time.fixedUnscaledDeltaTime));
        }

        private void OnApplicationQuit()
        {
            PBEvents.ApplicationQuitSubject.Publish();
        }
    }
}
#endif