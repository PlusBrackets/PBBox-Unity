/*--------------------------------------------------------
 *Copyright (c) 2022 PlusBrackets
 *@update: 2022.04.16
 *@author: PlusBrackets
 --------------------------------------------------------*/
#if (ODIN_INSPECTOR || ODIN_INSPECTOR_3) && UNITY_EDITOR
#define USE_ODIN
#endif
#if USE_ODIN
using Sirenix.OdinInspector;
#endif
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Serialization;

namespace PBBox
{

    // List<T>
    [System.Serializable]
    public class SList<T>
    {
        [SerializeField]
        List<T> values;
        public List<T> source => values;
        public T this[int index]
        {
            get
            {
                return values[index];
            }
        }
        public int Count
        {
            get
            {
                return values.Count;
            }
        }

        public SList(List<T> list = null)
        {
            if (list == null)
            {
                this.values = new List<T>();
            }
            else
            {
                this.values = list;
            }
        }
    }

}