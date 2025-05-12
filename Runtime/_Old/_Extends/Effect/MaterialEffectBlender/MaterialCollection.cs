/*--------------------------------------------------------
 *Copyright (c) 2016-2022 PlusBrackets
 *@update: 2022.11.09
 *@author: PlusBrackets
 --------------------------------------------------------*/
using System;
using System.Collections.Generic;
using UnityEngine;

namespace PBBox.Effects
{
    /// <summary>
    /// 材质集合
    /// </summary>
    public class MaterialCollection
    {
        public GameObject Owner { get; private set; }

        public class RendererMatInfo
        {
            public string shaderName;
            public Renderer renderer;
            public int index;
            // public string materialTags; //TODO 材质tag，用于识别属于什么材质。现版本暂不实现
            private MaterialPropertyBlock _blendingBlock;
            private Lazy<HashSet<string>> _vaildParams = new Lazy<HashSet<string>>();

            public Material GetMaterial()
            {
                return renderer.sharedMaterials[index];
            }

            public bool HasParam(string paramName)
            {
                return _vaildParams.IsValueCreated ?  _vaildParams.Value.Contains(paramName) : false;
            }

            #region Param Getter Setter
            public void SetColor(string paramName, Color color)
            {
                var bk = GetBlendingBlock();
                bk.SetColor(paramName, color);
                _vaildParams.Value.Add(paramName + "$Color");
            }

            public Color GetColor(string paramName)
            {
                if (HasParam(paramName + "$Color"))
                    return GetBlendingBlock().GetColor(paramName);
                else
                    return GetMaterial().GetColor(paramName);
            }

            public void SetFloat(string paramName, float value)
            {
                var bk = GetBlendingBlock();
                bk.SetFloat(paramName, value);
                _vaildParams.Value.Add(paramName + "$Float");
            }

            public float GetFloat(string paramName)
            {
                if (HasParam(paramName + "$Float"))
                    return GetBlendingBlock().GetFloat(paramName);
                else
                    return GetMaterial().GetFloat(paramName);
            }
            #endregion

            internal MaterialPropertyBlock GetBlendingBlock()
            {
                if (_blendingBlock == null)
                {
                    _blendingBlock = new MaterialPropertyBlock();
                    renderer.GetPropertyBlock(_blendingBlock, index);
                }
                return _blendingBlock;
            }

            internal void ResetBlendingBlock()
            {
                renderer.SetPropertyBlock(null, index);
                if(_vaildParams.IsValueCreated){
                    _vaildParams.Value.Clear();
                }
                if (_blendingBlock != null)
                {
                    renderer.GetPropertyBlock(_blendingBlock, index);
                }
            }

            internal void ApplyBlendingBlock()
            {
                renderer.SetPropertyBlock(_blendingBlock, index);
            }
        }

        public RendererMatInfo this[int index]
        {
            get
            {
                return _rendererMatInfos[index];
            }
        }

        public int Count => _rendererMatInfos.Count;

        /// <summary>
        /// 所有材质信息
        /// </summary>
        private List<RendererMatInfo> _rendererMatInfos;
        /// <summary>
        /// shader名表
        /// </summary>
        private Dictionary<string, List<int>> _shaderNameLookup;

        public MaterialCollection(GameObject Owner)
        {
            _rendererMatInfos = new List<RendererMatInfo>();
            _shaderNameLookup = new Dictionary<string, List<int>>();
            RebuildCollection(Owner);
        }

        public MaterialCollection(){
            _rendererMatInfos = new List<RendererMatInfo>();
            _shaderNameLookup = new Dictionary<string, List<int>>();
        }

        /// <summary>
        /// 重建Owner的Renderer和Material集合
        /// </summary>
        public void RebuildCollection(){
            RebuildCollection(Owner);
        }

        internal void ResetAllBlendingBlock()
        {
            if (_rendererMatInfos == null)
                return;
            foreach (var info in _rendererMatInfos)
            {
                info.ResetBlendingBlock();
            }
        }

        internal void ApplyAllBlendingBlock()
        {
            if (_rendererMatInfos == null)
                return;
            foreach (var info in _rendererMatInfos)
            {
                info.ApplyBlendingBlock();
            }
        }

        /// <summary>
        /// 重建Owner的Renderer和Material集合,会重新设置owner
        /// </summary>
        public void RebuildCollection(GameObject owner)
        {
            _rendererMatInfos.Clear();
            _shaderNameLookup.Clear();
            if (!owner)
                return;
            this.Owner = owner;
            Renderer[] renderers = owner.GetComponentsInChildren<Renderer>();
            foreach (var rd in renderers)
            {
                var smats = rd.sharedMaterials;//使用sharedMaterials避免访问Renderer时复制material
                for (int i = 0; i < smats.Length; i++)
                {
                    RendererMatInfo info = new RendererMatInfo();
                    info.renderer = rd;
                    info.index = i;
                    info.shaderName = smats[i].shader.name;
                    
                    _rendererMatInfos.Add(info);
                    if (!_shaderNameLookup.TryGetValue(info.shaderName, out var indexList))
                    {
                        indexList = new List<int>();
                        _shaderNameLookup.Add(info.shaderName, indexList);
                    }
                    indexList.Add(_rendererMatInfos.Count - 1);
                }
            }
        }

        /// <summary>
        /// 根据shaderName遍历所有RendererMatInfo
        /// </summary>
        /// <param name="shaderName"></param>
        /// <param name="handler"></param>
        public void ForeachRendererByShaderName(string shaderName, Action<RendererMatInfo> handler)
        {
            if (!_shaderNameLookup.TryGetValue(shaderName, out var indexs))
                return;
            foreach (int index in indexs)
            {
                handler.Invoke(_rendererMatInfos[index]);
            }
        }


    }
}