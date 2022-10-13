//Define USE Odin
#if (ODIN_INSPECTOR || ODIN_INSPECTOR_3) && UNITY_EDITOR
#define USE_ODIN
#endif
#if USE_ODIN
using Sirenix.OdinInspector;
#endif
//Define End
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace PBBox
{
#if USE_ODIN
    [InlineProperty]
#endif
    [Serializable, Tooltip("复数tag，使用英文分号\',\'分隔")]
    public class MutiTags : ISerializationCallbackReceiver
    {
        public static StringComparison DEFAULT_STRING_COMPARISON = StringComparison.OrdinalIgnoreCase;
        //默认分割字符
        const char DEFAULT_SPILT_CHAR = ',';
#if USE_ODIN
        [HideLabel]
#endif
        [SerializeField]
        string m_Tags = string.Empty;
        private Lazy<List<string>> _tagCache = new Lazy<List<string>>();
        private bool _shouldRefreshCache = false;

        public MutiTags(string tags)
        {
            SetTags(tags);
        }

        public MutiTags(){
            
        }

        public string[] ToArray()
        {
            return m_Tags.Split(',', StringSplitOptions.RemoveEmptyEntries);
        }

        /// <summary>
        /// 获取tag列表，会创建List<string>一次，用于复数匹配时提升性能
        /// </summary>
        /// <returns></returns>
        public List<string> GetCachedArray()
        {
            if (_shouldRefreshCache || !_tagCache.IsValueCreated)
            {
                _tagCache.Value.Clear();
                _tagCache.Value.AddRange(ToArray());
                _shouldRefreshCache = false;
            }
            return _tagCache.Value;
        }

        public void SetTags(string tagString){
            
            m_Tags = tagString;
            if (!string.IsNullOrEmpty(m_Tags) && !m_Tags.EndsWith(DEFAULT_SPILT_CHAR))
            {
                m_Tags += DEFAULT_SPILT_CHAR;
            }
            _shouldRefreshCache = true;
        }

        public void SetTags(params string[] tags)
        {
            string _tags = string.Join(',', tags);
            SetTags(_tags);
        }

        public void SetTags(IEnumerable<string> tags)
        {
            string _tags = string.Join(',', tags);
            SetTags(_tags);
        }

        /// <summary>
        /// 添加Tag
        /// </summary>
        /// <param name="tag"></param>
        public void AddTag(string tag)
        {
            AddTag(tag, DEFAULT_STRING_COMPARISON);
        }

        /// <summary>
        /// 添加Tag
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="stringComparison">比较方式</param>
        public void AddTag(string tag, StringComparison stringComparison)
        {
            string _tag = tag + DEFAULT_SPILT_CHAR;
            if (m_Tags.Contains(_tag, DEFAULT_STRING_COMPARISON))
            {
                return;
            }
            m_Tags += _tag;
            _shouldRefreshCache = true;
        }

        /// <summary>
        /// 添加tag
        /// </summary>
        /// <param name="tags"></param>
        public void AddTags(IEnumerable<string> tags)
        {
            AddTags(tags, DEFAULT_STRING_COMPARISON);
        }

        /// <summary>
        /// 添加Tag
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="stringComparison">比较方式</param>
        public void AddTags(IEnumerable<string> tags, StringComparison stringComparison)
        {
            foreach (var t in tags)
            {
                AddTag(t, stringComparison);
            }
        }

        /// <summary>
        /// 添加Tag
        /// </summary>
        /// <param name="tag"></param>
        public void AddTags(params string[] tags)
        {
            AddTags(DEFAULT_STRING_COMPARISON, tags);
        }

        /// <summary>
        /// 添加Tag
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="stringComparison">比较方式</param>
        public void AddTags(StringComparison stringComparison, params string[] tags)
        {
            foreach (var t in tags)
            {
                AddTag(t, stringComparison);
            }
        }

        /// <summary>
        /// 移除tag
        /// </summary>
        /// <param name="tag"></param>
        public void RemoveTag(string tag)
        {
            RemoveTag(tag, DEFAULT_STRING_COMPARISON);
        }

        /// <summary>
        /// 移除tag
        /// </summary>
        /// <param name="tag"></param>
        public void RemoveTag(string tag, StringComparison stringComparison)
        {
            m_Tags = m_Tags.Replace(tag + DEFAULT_SPILT_CHAR, "", stringComparison);
            _shouldRefreshCache = true;
        }

        /// <summary>
        /// 移除tags
        /// </summary>
        /// <param name="tags"></param>
        public void RemoveTags(params string[] tags)
        {
            RemoveTags(DEFAULT_STRING_COMPARISON, tags);
        }

        /// <summary>
        /// 移除tags
        /// </summary>
        /// <param name="tags"></param>
        public void RemoveTags(StringComparison stringComparison, params string[] tags)
        {
            foreach (var t in tags)
            {
                RemoveTag(t, stringComparison);
            }
        }

        /// <summary>
        /// 移除tags
        /// </summary>
        /// <param name="tags"></param>
        public void RemoveTags(IEnumerable<string> tags)
        {
            RemoveTags(tags, DEFAULT_STRING_COMPARISON);
        }

        /// <summary>
        /// 移除tags
        /// </summary>
        /// <param name="tags"></param>
        public void RemoveTags(IEnumerable<string> tags, StringComparison stringComparison)
        {
            foreach (var t in tags)
            {
                RemoveTag(t, stringComparison);
            }
        }

        /// <summary>
        /// 是否包含Tag
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        public bool HasTag(string tag)
        {
            return HasTag(tag, DEFAULT_STRING_COMPARISON);
        }

        /// <summary>
        /// 是否包含Tag
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        public bool HasTag(string tag, StringComparison stringComparison)
        {
            bool result = m_Tags.Contains(tag + DEFAULT_SPILT_CHAR, stringComparison);
            // DebugUtils.Test.Log($"Check Tag{tag} in {m_Tags}, {result}");
            return result;
        }

        /// <summary>
        /// 是否包含给定的所有tag，区分大小写
        /// </summary>
        /// <param name="tags"></param>
        /// <returns></returns>
        public bool HasAllTags(params string[] tags)
        {
            return HasAllTags(DEFAULT_STRING_COMPARISON, tags);
        }

        /// <summary>
        /// 是否包含给定的所有tag，区分大小写
        /// </summary>
        /// <param name="tags"></param>
        /// <returns></returns>
        public bool HasAllTags(StringComparison stringComparison, params string[] tags)
        {
            foreach (var t in tags)
            {
                if (!HasTag(t, stringComparison))
                    return false;
            }
            return true;
        }

        /// <summary>
        /// 是否包含给定的所有tag，区分大小写
        /// </summary>
        /// <param name="tags"></param>
        /// <returns></returns>
        public bool HasAllTags(IEnumerable<string> tags)
        {
            return HasAllTags(tags, DEFAULT_STRING_COMPARISON);
        }

        /// <summary>
        /// 是否包含给定的所有tag，区分大小写
        /// </summary>
        /// <param name="tags"></param>
        /// <returns></returns>
        public bool HasAllTags(IEnumerable<string> tags, StringComparison stringComparison)
        {
            foreach (var t in tags)
            {
                if (!HasTag(t, stringComparison))
                    return false;
            }
            return true;
        }

        /// <summary>
        /// 是否有任意tag
        /// </summary>
        /// <param name="tags"></param>
        /// <returns></returns>
        public bool HasAnyTags(params string[] tags)
        {
            return HasAnyTags(DEFAULT_STRING_COMPARISON, tags);
        }

        /// <summary>
        /// 是否有任意tag
        /// </summary>
        /// <param name="tags"></param>
        /// <returns></returns>
        public bool HasAnyTags(StringComparison stringComparison, params string[] tags)
        {
            foreach (var t in tags)
            {
                if (HasTag(t, stringComparison))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 是否有任意tag
        /// </summary>
        /// <param name="tags"></param>
        /// <returns></returns>
        public bool HasAnyTags(IEnumerable<string> tags)
        {
            return HasAnyTags(tags, DEFAULT_STRING_COMPARISON);
        }

        /// <summary>
        /// 是否有任意tag
        /// </summary>
        /// <param name="tags"></param>
        /// <returns></returns>
        public bool HasAnyTags(IEnumerable<string> tags, StringComparison stringComparison)
        {
            foreach (var t in tags)
            {
                if (HasTag(t, stringComparison))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 获得交集数量
        /// </summary>
        /// <param name="tags"></param>
        /// <returns></returns>
        public bool NumOfTags(out int num, params string[] tags)
        {
            return NumOfTags(out num, DEFAULT_STRING_COMPARISON, tags);
        }

        /// <summary>
        /// 获得交集数量
        /// </summary>
        /// <param name="tags"></param>
        /// <returns></returns>
        public bool NumOfTags(out int num, StringComparison stringComparison, params string[] tags)
        {
            num = 0;
            foreach (var t in tags)
            {
                if (HasTag(t, stringComparison))
                {
                    num++;
                }
            }
            return num > 0;
        }

        /// <summary>
        /// 获得交集数量
        /// </summary>
        /// <param name="tags"></param>
        /// <returns></returns>
        public bool NumOfTags(out int num, IEnumerable<string> tags)
        {
            return NumOfTags(out num, tags, DEFAULT_STRING_COMPARISON);
        }

        /// <summary>
        /// 获得交集数量
        /// </summary>
        /// <param name="tags"></param>
        /// <returns></returns>
        public bool NumOfTags(out int num, IEnumerable<string> tags, StringComparison stringComparison)
        {
            num = 0;
            foreach (var t in tags)
            {
                if (HasTag(t, stringComparison))
                {
                    num++;
                }
            }
            return num > 0;
        }

        public static implicit operator bool(MutiTags tags)
        {
            return !string.IsNullOrEmpty(tags.m_Tags);
        }

        public override string ToString()
        {
            return m_Tags;
        }

        #region Serialization
        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            if (!string.IsNullOrEmpty(m_Tags) && !m_Tags.EndsWith(DEFAULT_SPILT_CHAR))
            {
                m_Tags += DEFAULT_SPILT_CHAR;
            }
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            if (!string.IsNullOrEmpty(m_Tags) && !m_Tags.EndsWith(DEFAULT_SPILT_CHAR))
            {
                m_Tags += DEFAULT_SPILT_CHAR;
            }
        }

        #endregion
    }
}