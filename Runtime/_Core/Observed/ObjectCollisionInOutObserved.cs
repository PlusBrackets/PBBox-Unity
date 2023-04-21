// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using System;

// namespace PBBox
// {
//     [DisallowMultipleComponent]
//     [AddComponentMenu("")]
//     public class ObjectCollisionInOutObserved : MonoBehaviour
//     {
//         SimpleObservable<GameObject> m_SubjectEnter;
//         SimpleObservable<GameObject> m_SubjectExit;

//         public SimpleObservable<GameObject> GetEnterObserved()
//         {
//             return m_SubjectEnter ?? (m_SubjectEnter = new SimpleObservable<GameObject>());
//         }

//         public SimpleObservable<GameObject> GetExitObserved()
//         {
//             return m_SubjectExit ?? (m_SubjectExit = new SimpleObservable<GameObject>());
//         }

//         private void OnCollisionEnter(Collision other)
//         {
//             m_SubjectEnter?.OnNext(other.gameObject);
//         }

//         private void OnCollisionExit(Collision other)
//         {
//             m_SubjectExit?.OnNext(other.gameObject);
//         }
//     }

//     public static partial class SimpleObservedExtensions
//     {
//         /// <summary>
//         /// 观察Trigger Enter
//         /// </summary>
//         /// <param name="target"></param>
//         /// <param name="onNext"></param>
//         /// <param name="onComplete"></param>
//         /// <returns></returns>
//         public static SimpleObservable<GameObject>.Subscription ObservedCollisionEnter(this GameObject target, Action<GameObject> onNext, Action onComplete = null)
//         {
//             return target.GetOrAddComponent<ObjectCollisionInOutObserved>().GetEnterObserved().Subscribe(onNext, onComplete);
//         }

//         /// <summary>
//         /// 观察Trigger Exit
//         /// </summary>
//         /// <param name="target"></param>
//         /// <param name="onNext"></param>
//         /// <param name="onComplete"></param>
//         /// <returns></returns>
//         public static SimpleObservable<GameObject>.Subscription ObservedCollisionExit(this GameObject target, Action<GameObject> onNext, Action onComplete = null)
//         {
//             return target.GetOrAddComponent<ObjectCollisionInOutObserved>().GetExitObserved().Subscribe(onNext, onComplete);
//         }
//     }
// }