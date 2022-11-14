/*--------------------------------------------------------
 *Copyright (c) 2022 PlusBrackets
 *@update: 2022.06.18
 *@author: PlusBrackets
 --------------------------------------------------------*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;

namespace PBBox
{

    public partial class PBEvents
    {
        public struct Register<TKey> where TKey : Enum
        {
            public static readonly Register<TKey> DEFAULT = new Register<TKey>();
            /// <summary>
            /// 添加监听
            /// </summary>
            /// <param name="key"></param>
            /// <param name="listener"></param>
            /// <param name="order">自定义执行顺序，默认为0</param>
            public void On(TKey key, Action listener, int order = 0)
            {
                PBEvents.On(key, listener, order);
            }

            /// <summary>
            /// 添加监听，只执行一次
            /// </summary>
            /// <param name="key"></param>
            /// <param name="listener"></param>
            /// <param name="order">自定义执行顺序，默认为0</param>
            public void Once(TKey key, Action listener, int order = 0)
            {
                PBEvents.On(key, listener, order);
            }

            /// <summary>
            /// 移除监听
            /// </summary>
            /// <param name="key"></param>
            /// <param name="listener"></param>
            /// <param name="order">若为空，则会按执行顺序从后到前移除一个listener，若不为空，则只会移除order相同的最后一个listener</param>
            public void Off<T>(TKey key, Action listener, int? order = null)
            {
                PBEvents.Off(key, listener, order);
            }
        }

        public struct Register<TKey, TParam> where TKey : Enum
        {
            public static readonly Register<TKey, TParam> DEFAULT = new Register<TKey, TParam>();
            /// <summary>
            /// 添加监听
            /// </summary>
            /// <param name="key"></param>
            /// <param name="listener"></param>
            /// <param name="order">自定义执行顺序，默认为0</param>
            public void On(TKey key, Action<TParam> listener, int order = 0)
            {
                PBEvents.On(key, listener, order);
            }
            /// <summary>
            /// 添加监听，只执行一次
            /// </summary>
            /// <param name="key"></param>
            /// <param name="listener"></param>
            /// <param name="order">自定义执行顺序，默认为0</param>
            public void Once(TKey key, Action<TParam> listener, int order = 0)
            {
                PBEvents.Once(key, listener, order);
            }

            /// <summary>
            /// 移除监听
            /// </summary>
            /// <param name="key"></param>
            /// <param name="listener"></param>
            /// <param name="order">若为空，则会按执行顺序从后到前移除一个listener，若不为空，则只会移除order相同的最后一个listener</param>
            public void Off(TKey key, Action<TParam> listener, int? order = null)
            {
                PBEvents.Off(key, listener, order);
            }

        }

        public struct Register<TKey, TParam1, TParam2> where TKey : Enum
        {
            public static readonly Register<TKey, TParam1, TParam2> DEFAULT = new Register<TKey, TParam1, TParam2>();
            /// <summary>
            /// 添加监听
            /// </summary>
            /// <param name="key"></param>
            /// <param name="listener"></param>
            /// <param name="order">自定义执行顺序，默认为0</param>
            public void On(TKey key, Action<TParam1, TParam2> listener, int order = 0)
            {
                PBEvents.On(key, listener, order);
            }
            /// <summary>
            /// 添加监听，只执行一次
            /// </summary>
            /// <param name="key"></param>
            /// <param name="listener"></param>
            /// <param name="order">自定义执行顺序，默认为0</param>
            public void Once(TKey key, Action<TParam1, TParam2> listener, int order = 0)
            {
                PBEvents.Once(key, listener, order);
            }

            /// <summary>
            /// 移除监听
            /// </summary>
            /// <param name="key"></param>
            /// <param name="listener"></param>
            /// <param name="order">若为空，则会按执行顺序从后到前移除一个listener，若不为空，则只会移除order相同的最后一个listener</param>
            public void Off(TKey key, Action<TParam1, TParam2> listener, int? order = null)
            {
                PBEvents.Off(key, listener, order);
            }

        }

        public struct Register<TKey, TParam1, TParam2, TParam3> where TKey : Enum
        {
            public static readonly Register<TKey, TParam1, TParam2, TParam3> DEFAULT = new Register<TKey, TParam1, TParam2, TParam3>();
            /// <summary>
            /// 添加监听
            /// </summary>
            /// <param name="key"></param>
            /// <param name="listener"></param>
            /// <param name="order">自定义执行顺序，默认为0</param>
            public void On(TKey key, Action<TParam1, TParam2, TParam3> listener, int order = 0)
            {
                PBEvents.On(key, listener, order);
            }
            /// <summary>
            /// 添加监听，只执行一次
            /// </summary>
            /// <param name="key"></param>
            /// <param name="listener"></param>
            /// <param name="order">自定义执行顺序，默认为0</param>
            public void Once(TKey key, Action<TParam1, TParam2, TParam3> listener, int order = 0)
            {
                PBEvents.Once(key, listener, order);
            }

            /// <summary>
            /// 移除监听
            /// </summary>
            /// <param name="key"></param>
            /// <param name="listener"></param>
            /// <param name="order">若为空，则会按执行顺序从后到前移除一个listener，若不为空，则只会移除order相同的最后一个listener</param>
            public void Off(TKey key, Action<TParam1, TParam2, TParam3> listener, int? order = null)
            {
                PBEvents.Off(key, listener, order);
            }

        }

        public struct Register<TKey, TParam1, TParam2, TParam3, TParam4> where TKey : Enum
        {
            public static readonly Register<TKey, TParam1, TParam2, TParam3, TParam4> DEFAULT = new Register<TKey, TParam1, TParam2, TParam3, TParam4>();
            /// <summary>
            /// 添加监听
            /// </summary>
            /// <param name="key"></param>
            /// <param name="listener"></param>
            /// <param name="order">自定义执行顺序，默认为0</param>
            public void On(TKey key, Action<TParam1, TParam2, TParam3, TParam4> listener, int order = 0)
            {
                PBEvents.On(key, listener, order);
            }
            /// <summary>
            /// 添加监听，只执行一次
            /// </summary>
            /// <param name="key"></param>
            /// <param name="listener"></param>
            /// <param name="order">自定义执行顺序，默认为0</param>
            public void Once(TKey key, Action<TParam1, TParam2, TParam3, TParam4> listener, int order = 0)
            {
                PBEvents.Once(key, listener, order);
            }

            /// <summary>
            /// 移除监听
            /// </summary>
            /// <param name="key"></param>
            /// <param name="listener"></param>
            /// <param name="order">若为空，则会按执行顺序从后到前移除一个listener，若不为空，则只会移除order相同的最后一个listener</param>
            public void Off(TKey key, Action<TParam1, TParam2, TParam3, TParam4> listener, int? order = null)
            {
                PBEvents.Off(key, listener, order);
            }

        }
    
         public struct ObjKeyRegister<TKey> 
        {
            public static readonly ObjKeyRegister<TKey> DEFAULT = new ObjKeyRegister<TKey>();
            /// <summary>
            /// 添加监听
            /// </summary>
            /// <param name="key"></param>
            /// <param name="listener"></param>
            /// <param name="order">自定义执行顺序，默认为0</param>
            public void On(TKey key, Action listener, int order = 0)
            {
                PBEvents.On(key, listener, order);
            }

            /// <summary>
            /// 添加监听，只执行一次
            /// </summary>
            /// <param name="key"></param>
            /// <param name="listener"></param>
            /// <param name="order">自定义执行顺序，默认为0</param>
            public void Once(TKey key, Action listener, int order = 0)
            {
                PBEvents.On(key, listener, order);
            }

            /// <summary>
            /// 移除监听
            /// </summary>
            /// <param name="key"></param>
            /// <param name="listener"></param>
            /// <param name="order">若为空，则会按执行顺序从后到前移除一个listener（若有相同listener），若不为空，则只会移除order相同的最后一个listener</param>
            public void Off<T>(TKey key, Action listener, int? order = null)
            {
                PBEvents.Off(key, listener, order);
            }
        }

        public struct ObjKeyRegister<TKey, TParam>
        {
            public static readonly ObjKeyRegister<TKey, TParam> DEFAULT = new ObjKeyRegister<TKey, TParam>();
            /// <summary>
            /// 添加监听
            /// </summary>
            /// <param name="key"></param>
            /// <param name="listener"></param>
            /// <param name="order">自定义执行顺序，默认为0</param>
            public void On(TKey key, Action<TParam> listener, int order = 0)
            {
                PBEvents.On(key, listener, order);
            }
            /// <summary>
            /// 添加监听，只执行一次
            /// </summary>
            /// <param name="key"></param>
            /// <param name="listener"></param>
            /// <param name="order">自定义执行顺序，默认为0</param>
            public void Once(TKey key, Action<TParam> listener, int order = 0)
            {
                PBEvents.Once(key, listener, order);
            }

            /// <summary>
            /// 移除监听
            /// </summary>
            /// <param name="key"></param>
            /// <param name="listener"></param>
            /// <param name="order">若为空，则会按执行顺序从后到前移除一个listener，若不为空，则只会移除order相同的最后一个listener</param>
            public void Off(TKey key, Action<TParam> listener, int? order = null)
            {
                PBEvents.Off(key, listener, order);
            }

        }

        public struct ObjKeyRegister<TKey, TParam1, TParam2>
        {
            public static readonly ObjKeyRegister<TKey, TParam1, TParam2> DEFAULT = new ObjKeyRegister<TKey, TParam1, TParam2>();
            /// <summary>
            /// 添加监听
            /// </summary>
            /// <param name="key"></param>
            /// <param name="listener"></param>
            /// <param name="order">自定义执行顺序，默认为0</param>
            public void On(TKey key, Action<TParam1, TParam2> listener, int order = 0)
            {
                PBEvents.On(key, listener, order);
            }
            /// <summary>
            /// 添加监听，只执行一次
            /// </summary>
            /// <param name="key"></param>
            /// <param name="listener"></param>
            /// <param name="order">自定义执行顺序，默认为0</param>
            public void Once(TKey key, Action<TParam1, TParam2> listener, int order = 0)
            {
                PBEvents.Once(key, listener, order);
            }

            /// <summary>
            /// 移除监听
            /// </summary>
            /// <param name="key"></param>
            /// <param name="listener"></param>
            /// <param name="order">若为空，则会按执行顺序从后到前移除一个listener，若不为空，则只会移除order相同的最后一个listener</param>
            public void Off(TKey key, Action<TParam1, TParam2> listener, int? order = null)
            {
                PBEvents.Off(key, listener, order);
            }

        }

        public struct ObjKeyRegister<TKey, TParam1, TParam2, TParam3>
        {
            public static readonly ObjKeyRegister<TKey, TParam1, TParam2, TParam3> DEFAULT = new ObjKeyRegister<TKey, TParam1, TParam2, TParam3>();
            /// <summary>
            /// 添加监听
            /// </summary>
            /// <param name="key"></param>
            /// <param name="listener"></param>
            /// <param name="order">自定义执行顺序，默认为0</param>
            public void On(TKey key, Action<TParam1, TParam2, TParam3> listener, int order = 0)
            {
                PBEvents.On(key, listener, order);
            }
            /// <summary>
            /// 添加监听，只执行一次
            /// </summary>
            /// <param name="key"></param>
            /// <param name="listener"></param>
            /// <param name="order">自定义执行顺序，默认为0</param>
            public void Once(TKey key, Action<TParam1, TParam2, TParam3> listener, int order = 0)
            {
                PBEvents.Once(key, listener, order);
            }

            /// <summary>
            /// 移除监听
            /// </summary>
            /// <param name="key"></param>
            /// <param name="listener"></param>
            /// <param name="order">若为空，则会按执行顺序从后到前移除一个listener，若不为空，则只会移除order相同的最后一个listener</param>
            public void Off(TKey key, Action<TParam1, TParam2, TParam3> listener, int? order = null)
            {
                PBEvents.Off(key, listener, order);
            }

        }

        public struct ObjKeyRegister<TKey, TParam1, TParam2, TParam3, TParam4>
        {
            public static readonly ObjKeyRegister<TKey, TParam1, TParam2, TParam3, TParam4> DEFAULT = new ObjKeyRegister<TKey, TParam1, TParam2, TParam3, TParam4>();
            /// <summary>
            /// 添加监听
            /// </summary>
            /// <param name="key"></param>
            /// <param name="listener"></param>
            /// <param name="order">自定义执行顺序，默认为0</param>
            public void On(TKey key, Action<TParam1, TParam2, TParam3, TParam4> listener, int order = 0)
            {
                PBEvents.On(key, listener, order);
            }
            /// <summary>
            /// 添加监听，只执行一次
            /// </summary>
            /// <param name="key"></param>
            /// <param name="listener"></param>
            /// <param name="order">自定义执行顺序，默认为0</param>
            public void Once(TKey key, Action<TParam1, TParam2, TParam3, TParam4> listener, int order = 0)
            {
                PBEvents.Once(key, listener, order);
            }

            /// <summary>
            /// 移除监听
            /// </summary>
            /// <param name="key"></param>
            /// <param name="listener"></param>
            /// <param name="order">若为空，则会按执行顺序从后到前移除一个listener，若不为空，则只会移除order相同的最后一个listener</param>
            public void Off(TKey key, Action<TParam1, TParam2, TParam3, TParam4> listener, int? order = null)
            {
                PBEvents.Off(key, listener, order);
            }

        }
    
    }

}