using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PBBox.UI
{
    public abstract class UIAdapterList<Data> : MonoBehaviour
    {
        [SerializeField,Tooltip("列表子项预制件，需要有继承UIAdapterItem的组件")]
        protected GameObject m_ItemPrefab;
        [SerializeField]
        protected Transform m_ItemContain;
        public Transform itemContain => m_ItemContain;
        public List<Data> datas { get; protected set; }
        public List<UIAdapterItem<Data>> items { get; protected set; }
        protected SimplePool m_ItemPool;
        private int m_Selected = -1;
        public int selected
        {
            get => m_Selected;
            set
            {
                if (m_Selected != value)
                {
                    var item = selectedItem;
                    if (item != null)
                    {
                        item.selected = false;
                        item.OnDeselect();
                    }
                    this.m_Selected = value;
                    item = selectedItem;
                    if (item != null && item.IsSelectable())
                    {
                        item.selected = true;
                        item.OnSelected();
                    }
                    else
                        this.m_Selected = -1;
                    OnSelectedChanged(this.m_Selected);
                }
            }
        }
        public UIAdapterItem<Data> selectedItem
        {
            get
            {
                if (items != null && items.Count > m_Selected && m_Selected >= 0)
                    return items[m_Selected];
                return null;
            }
        }

        protected virtual void Awake()
        {
            items = new List<UIAdapterItem<Data>>();
            m_ItemPool = new SimplePool(m_ItemPrefab, itemContain);
        }

        // protected virtual void OnDestroy()
        // {
            // m_ItemPool.Clear();
        // }

        public void UpdateData(List<Data> datas)
        {
            this.datas = datas;
            this.RefreshItems();
        }

        protected virtual void RefreshItems()
        {
            int dataCount = datas != null ? datas.Count : 0;
            for (int i = 0; i < dataCount; i++)
            {
                Data d = datas[i];
                if (i >= items.Count)
                {
                    var item = m_ItemPool.Spawn();
                    items.Add(item.GetComponent<UIAdapterItem<Data>>());
                }
                items[i].SetData(d, this, i);
            }
            for (int j = items.Count - 1; j >= dataCount; j--)
            {
                if (selected == j)
                    selected = -1;
                UIAdapterItem<Data> item = CommonUtils.PopList(items);
                item.SetData(default(Data), null, -1);
                m_ItemPool.Recycle(item.gameObject);
            }
        }

        protected virtual void OnSelectedChanged(int selected){

        }
    }
}