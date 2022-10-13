
/*--------------------------------------------------------
 *Copyright (c) 2022 PlusBrackets
 *@update: 2022.05.21
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

namespace PBBox.UI
{
    public abstract class UIAdapterListComponent<TData> : MonoBehaviour
    {
#if USE_ODIN
        [InlineProperty, HideLabel, FoldoutGroup("Adapter List")]
#endif
        [SerializeField]
        UIAdapterList<TData> m_AdapterList;

        protected virtual void OnDestroy()
        {
            m_AdapterList?.Dispose();
        }
    }

    public interface IUIAdapterList
    {
        void InvokeItemEvent(int index, string key);
        void SelectItem(int index);
    }

    //TODO  添加UIAdapterParentItem，用于作为不同数据匹配item的中转 //(过时)添加支持多个Prefab，用于适配不同数据类型
    [System.Serializable]
    public class UIAdapterList<TData> : System.IDisposable, IUIAdapterList
    {
        [SerializeField, Tooltip("列表子项预制件，需要有继承UIAdapterItem的组件")]
        protected GameObject m_ItemPrefab;
        public GameObject ItemPrefab => m_ItemPrefab;
        [SerializeField]
        protected Transform m_ItemContain;
        public Transform ItemContain => m_ItemContain;
        
        public List<TData> Datas { get; protected set; }
        // public List<UIAdapterItem<TData>> items { get; protected set; }
        
        public List<IUIAdapterItem> Items { get; protected set; }
        protected SimplePool m_ItemPool;
        int m_Selected = -1;
        public int Selected
        {
            get => m_Selected;
            set
            {
                if (m_Selected != value)
                {
                    var item = SelectedItem;
                    if (item != null)
                    {
                        // item.selected = false;
                        item.Deselect();
                    }
                    this.m_Selected = value;
                    item = SelectedItem;
                    if (item != null && item.IsSelectable())
                    {
                        // item.selected = true;
                        item.Select();
                    }
                    else
                        this.m_Selected = -1;
                    OnSelectedChanged(this.m_Selected);
                }
            }
        }

        public bool IsDisposed { get; private set; } = false;

        public IUIAdapterItem SelectedItem
        {
            get
            {
                if (Items != null && Items.Count > m_Selected && m_Selected >= 0)
                    return Items[m_Selected];
                return null;
            }
        }

        public event System.Action<int, string> onItemEvent;

        public UIAdapterList(GameObject itemPrefab, Transform itemContain){
            m_ItemPrefab = itemPrefab;
            m_ItemContain = itemContain;
            Items = new List<IUIAdapterItem>();
            m_ItemPool = new SimplePool(m_ItemPrefab, itemContain);
        }

        ~UIAdapterList()
        {
            Dispose(false);
        }

        public void SetData(IEnumerable<TData> datas){
            this.Datas = new List<TData>(datas);
            SetData(this.Datas);
        }

        public void SetData(List<TData> datas)
        {
            this.Datas = datas;
            this.RefreshItems();
        }

        protected virtual void RefreshItems()
        {
            int dataCount = Datas != null ? Datas.Count : 0;
            for (int i = 0; i < dataCount; i++)
            {
                TData d = Datas[i];
                if (i >= Items.Count)
                {
                    var item = m_ItemPool.Spawn();
                    Items.Add(item.GetComponent<IUIAdapterItem>());
                }
                Items[i].SetData(d, this, i);
            }
            for (int j = Items.Count - 1; j >= dataCount; j--)
            {
                if (Selected == j)
                    Selected = -1;
                IUIAdapterItem item = CommonUtils.PopList(Items);

                item.ClearData();
                item.RecycleSelf();// m_ItemPool.Recycle(item.GetGameObject());
            }
        }

        protected virtual void OnSelectedChanged(int selected) { }

        protected virtual void OnItemEvent(int index, string eventKey) { }

        void IUIAdapterList.SelectItem(int index){
            Selected = index;
        }

        void IUIAdapterList.InvokeItemEvent(int index, string key)
        {
            if (index >= 0 && index < Items.Count)
            {
                OnItemEvent(index, key);
                onItemEvent?.Invoke(index, key);
            }
        }

        private void Dispose(bool isDisposing)
        {
            if (IsDisposed)
                return;
            IsDisposed = true;
            if (isDisposing)
            {
                m_ItemPool?.Clean();
                m_ItemPool?.DestoryPool();
            }
            m_ItemPrefab = null;
            m_ItemContain = null;
            Items = null;
            Datas = null;
            m_ItemPool = null;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}