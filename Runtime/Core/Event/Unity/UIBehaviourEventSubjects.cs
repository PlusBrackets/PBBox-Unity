#if UNITY_5_3_OR_NEWER
/*--------------------------------------------------------
 *Copyright (c) 2016-2026 PlusBrackets
 *@update: 2026.11.17
 *@author: PlusBrackets
 --------------------------------------------------------*/
using UnityEngine;
using UnityEngine.EventSystems;
using PBBox.Event;

namespace PBBox.Unity.Event
{
    /// <summary>
    /// UIBehaviour 相关的事件订阅组件。
    /// 使用SimpleEventSubject处理事件，请避免频繁大量订阅和移除操作（耗时）。
    /// </summary>
    [ExecuteAlways]
    [DisallowMultipleComponent]
    [AddComponentMenu("PBBox/Events/UI Behaviour Event Subjects")]
    [RequireComponent(typeof(RectTransform))]
    public class UIBehaviourEventSubjects : UIBehaviour
    {
        private SimpleEventSubject<RectTransform> _onRectDimensionsChanged = null;

        public IEventSubject<RectTransform> OnRectDimensionsChanged
        {
            get
            {
                if (_onRectDimensionsChanged == null)
                {
                    _onRectDimensionsChanged = new SimpleEventSubject<RectTransform>();
                }
                return _onRectDimensionsChanged;
            }
        }

        protected override void OnRectTransformDimensionsChange()
        {
            base.OnRectTransformDimensionsChange();
            _onRectDimensionsChanged?.Publish(transform as RectTransform);
        }
    }
}

namespace PBBox
{
    using PBBox.Unity.Event;

    public static partial class PBExtensions
    {
        public static IEventSubject<RectTransform> OnRectDimensionsChangedEvent(this RectTransform transform)
        {
            return transform.GetOrAddComponent<UIBehaviourEventSubjects>().OnRectDimensionsChanged;
        }
    }
}
#endif