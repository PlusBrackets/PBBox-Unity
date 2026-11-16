#if UNITY_5_3_OR_NEWER
/*--------------------------------------------------------
 *Copyright (c) 2016-2026 PlusBrackets
 *@update: 2026.11.17
 *@author: PlusBrackets
 --------------------------------------------------------*/
using System;
using UnityEngine;

namespace PBBox.Event
{
    [DisallowMultipleComponent]
    [AddComponentMenu("PBBox/Events/Object Life Events")]
    public class ObjectLifeEvents : MonoBehaviour
    {
        private SimpleEventSubject<GameObject> _onEnabled = null;
        private SimpleEventSubject<GameObject> _onDisabled = null;
        private SimpleEventSubject<GameObject> _onDestroyed = null;

        public IEventSubject<GameObject> OnEnabled
        {
            get
            {
                if (_onEnabled == null)
                {
                    _onEnabled = new SimpleEventSubject<GameObject>();
                }
                return _onEnabled;
            }
        }
        public IEventSubject<GameObject> OnDisabled
        {
            get
            {
                if (_onDisabled == null)
                {
                    _onDisabled = new SimpleEventSubject<GameObject>();
                }
                return _onDisabled;
            }
        }
        public IEventSubject<GameObject> OnDestroyed
        {
            get
            {
                if (_onDestroyed == null)
                {
                    _onDestroyed = new SimpleEventSubject<GameObject>();
                }
                return _onDestroyed;
            }
        }

        private void OnEnable()
        {
            _onEnabled?.Publish(gameObject);
        }

        private void OnDisable()
        {
            _onDisabled?.Publish(gameObject);
        }

        private void OnDestroy()
        {
            _onDestroyed?.Publish(gameObject);
        }

    }
}

namespace PBBox
{
    using PBBox.Event;

    public static partial class PBExtensions
    {
        public static EventSubscription SubscribeEnabled(this GameObject gameObject, Action<GameObject> handler)
        {
            return gameObject.GetOrAddComponent<ObjectLifeEvents>().OnEnabled.Subscribe(handler);
        }

        public static EventSubscription SubscribeDisabled(this GameObject gameObject, Action<GameObject> handler)
        {
            return gameObject.GetOrAddComponent<ObjectLifeEvents>().OnDisabled.Subscribe(handler);
        }

        public static EventSubscription SubscribeDestroyed(this GameObject gameObject, Action<GameObject> handler)
        {
            return gameObject.GetOrAddComponent<ObjectLifeEvents>().OnDestroyed.Subscribe(handler);
        }

        public static void UnsubscribeEnabled(this GameObject gameObject, Action<GameObject> handler)
        {
            var lifeEvents = gameObject.GetComponent<ObjectLifeEvents>();
            if (lifeEvents != null)
            {
                lifeEvents.OnEnabled.Unsubscribe(handler);
            }
        }

        public static void UnsubscribeDisabled(this GameObject gameObject, Action<GameObject> handler)
        {
            var lifeEvents = gameObject.GetComponent<ObjectLifeEvents>();
            if (lifeEvents != null)
            {
                lifeEvents.OnDisabled.Unsubscribe(handler);
            }
        }

        public static void UnsubscribeDestroyed(this GameObject gameObject, Action<GameObject> handler)
        {
            var lifeEvents = gameObject.GetComponent<ObjectLifeEvents>();
            if (lifeEvents != null)
            {
                lifeEvents.OnDestroyed.Unsubscribe(handler);
            }
        }
    }
}
#endif