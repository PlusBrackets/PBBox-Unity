/*--------------------------------------------------------
 *Copyright (c) 2016-2023 PlusBrackets
 *@update: 2023.02.23
 *@author: PlusBrackets
 --------------------------------------------------------*/
using System;
using System.Reflection;

namespace PBBox.Attributes
{
    /// <summary>
    /// 给一些利用反射获取数据的工具提供自定义的显示功能
    /// </summary>
    [AttributeUsageAttribute(AttributeTargets.All, Inherited = false, AllowMultiple = false)]
    public class CustomReflectDisplayAttribute : Attribute
    {
        private string m_Name = null;
        private bool m_ShowFullTypeName = true;
        private int m_Order = 0;

        public CustomReflectDisplayAttribute(string displayName, int order = 0)
        {
            m_Name = displayName;
            m_Order = order;
        }

        public CustomReflectDisplayAttribute(bool showFullTypeName, int order = 0)
        {
            m_ShowFullTypeName = showFullTypeName;
            m_Order = order;
        }

        public CustomReflectDisplayAttribute(int order)
        {
            m_Order = order;
        }

        public string GetName(Type type)
        {
            return string.IsNullOrEmpty(m_Name) ? (m_ShowFullTypeName ? type.FullName : type.Name) : m_Name;
        }

        public int GetOrder(){
            return m_Order;
        }
    }

    public static partial class PBExtensions
    {
        /// <summary>
        /// 若该类型有使用CustomReflectDisplayerAttribute特性，可以返回指定的显示名称
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static bool TryGetCustomDisplayName(this Type type, out string name)
        {
            int _order = 0;
            return TryGetCustomDisplayName(type, out name, ref _order);
        }

        public static bool TryGetCustomDisplayName(this Type type, out string name, ref int order)
        {
            name = null;
            if (type.IsDefined(typeof(CustomReflectDisplayAttribute), false))
            {
                CustomReflectDisplayAttribute _customDisplay = type.GetCustomAttribute(typeof(CustomReflectDisplayAttribute), false) as CustomReflectDisplayAttribute;
                if (_customDisplay != null)
                {
                    name = _customDisplay.GetName(type);
                    order = _customDisplay.GetOrder();
                    return true;
                }
            }
            return false;
        }
    }

}