/*--------------------------------------------------------
 *Copyright (c) 2016-2023 PlusBrackets
 *@update: 2023.06.20
 *@author: PlusBrackets
 --------------------------------------------------------*/
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PBBox.Collections
{
    /// <summary>
    /// Content添加了SerializeReference特性的WeightedItem
    /// </summary>
    [Serializable]
    public struct ReferenceWeightedItem<TContent> : IWeightedItem<TContent>, IEquatable<ReferenceWeightedItem<TContent>>
    {
        [SerializeField, SerializeReference]
        private TContent m_Content;
        [SerializeField, Range(0f, 100f)]
        private float m_Weights;

        public TContent Content => m_Content;
        public float Weights { get => m_Weights; set => m_Weights = value; }

        public bool Equals(ReferenceWeightedItem<TContent> other)
        {
            return EqualityComparer<TContent>.Default.Equals(m_Content, other.Content) && m_Weights - other.m_Weights < float.Epsilon;
        }
    }
}