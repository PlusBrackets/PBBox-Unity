/*--------------------------------------------------------
 *Copyright (c) 2016-2023 PlusBrackets
 *@update: 2023.04.21
 *@author: PlusBrackets
 --------------------------------------------------------*/
using System;
using System.Collections;
using System.Collections.Generic;

namespace PBBox.Collections
{
    [Serializable]
    public struct WeightedItem<TContent> : IWeightedItem<TContent>, IEquatable<WeightedItem<TContent>>
    {
#if UNITY_5_3_OR_NEWER
        [UnityEngine.SerializeField]
#endif
        private TContent m_Content;
#if UNITY_5_3_OR_NEWER
        [UnityEngine.SerializeField, UnityEngine.Range(0f, 100f)]
#endif
        private float m_Weights;

        public TContent Content => m_Content;
        public float Weights { get => m_Weights; set => m_Weights = value; }

        public bool Equals(WeightedItem<TContent> other)
        {
            return EqualityComparer<TContent>.Default.Equals(m_Content, other.Content) && m_Weights - other.m_Weights < float.Epsilon;
        }
    }
}