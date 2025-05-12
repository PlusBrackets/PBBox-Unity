using System.ComponentModel;
/*--------------------------------------------------------
 *Copyright (c) 2016-2023 PlusBrackets
 *@update: 2023.02.22
 *@author: PlusBrackets
 --------------------------------------------------------*/
using System.Diagnostics;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

namespace PBBox.Attributes
{
    /// <summary>
    /// 给[SerializeReference]特性提供一个类型选择器
    /// </summary>
    [Conditional("UNITY_EDITOR")]
    [AttributeUsageAttribute(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public sealed partial class SerializeRefSelectorAttribute : PropertyAttribute
    {
        private static readonly string[] DEFAULT_ASSEMBLIES = new string[] { "Assembly-CSharp", "PBBox" };
        private string[] m_FromAssemblies = DEFAULT_ASSEMBLIES;
        private Type m_FromType = null;
        private Type[] m_IncludeTypes = null;

        private Type[] m_SelectableTypes = null;
        // private string[] m_SelectableTypeNames = null;
        public Type[] SelectableTypes
        {
            get
            {
                if (m_SelectableTypes != null)
                {
                    return m_SelectableTypes;
                }
                var _list = new HashSet<Type>();
                if (m_FromType != null)
                {
                    _list.UnionWith(m_FromType.GetReferencingClass(assemblyNames: m_FromAssemblies));
                }
                if (m_IncludeTypes != null)
                {
                    _list.UnionWith(m_IncludeTypes);
                }
                m_SelectableTypes = _list.ToArray();
                return m_SelectableTypes;
            }
        }

        public SerializeRefSelectorAttribute(Type fromType, Type[] includeTypes = null, string[] fromAssemblies = null)
        {
            m_FromType = fromType;
            m_IncludeTypes = includeTypes;
            m_FromAssemblies = fromAssemblies ?? DEFAULT_ASSEMBLIES;
        }

        public SerializeRefSelectorAttribute(Type[] includeTypes, string[] fromAssemblies = null)
        {
            m_FromType = null;
            m_IncludeTypes = includeTypes;
            m_FromAssemblies = fromAssemblies ?? DEFAULT_ASSEMBLIES;
        }
    }
}