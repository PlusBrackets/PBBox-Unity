using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PBBox.UI
{

    public interface IUIAdapterItem : IPoolObject
    {
        TData GetData<TData>();
        void SetData<TData>(TData data, IUIAdapterList list, int index);
        void ClearData();
        bool IsSelectable();
        void Select();
        void Deselect();
        // GameObject GetGameObject();
    }

    [AddComponentMenu("")]
    public abstract class UIAdapterItem<TData> : MonoBehaviour, IUIAdapterItem
    {
        SimplePool IPoolObject.Pool { get; set; }
        public TData Data { get; protected set; }
        // protected TData PreviourData { get; private set; }
        public IUIAdapterList AdapterList { get; private set; }
        public int Index { get; private set; } = 0;
        public bool Selected { get; protected set; } = false;

        public virtual bool IsSelectable()
        {
            return false;
        }

        T IUIAdapterItem.GetData<T>()
        {
            if (Data is T _data)
            {
                return _data;
            }
            return default(T);
        }

        void IUIAdapterItem.SetData<T>(T data, IUIAdapterList list, int index)
        {
            if (data is TData _data)
            {
                SetData(_data, list, index);
            }
            else
            {
                this.LogInfoError("数值类型不匹配");
            }
        }

        public virtual void SetData(TData data, IUIAdapterList list, int index)
        {
            OnClearData();
            this.Data = data;
            this.AdapterList = list;
            this.Index = index;
            OnDataUpdate(data);
            // this.PreviourData = default(TData);
        }

        public virtual void ClearData()
        {
            OnClearData();
            this.Data = default(TData);
            this.AdapterList = null;
            this.Index = -1;
        }

        void IUIAdapterItem.Select()
        {
            Selected = true;
            OnSelected();
        }

        void IUIAdapterItem.Deselect()
        {
            Selected = false;
            OnDeselected();
        }

        // GameObject IUIAdapterItem.GetGameObject()
        // {
        //     return gameObject;
        // }

        public void ToggleSelect()
        {
            if (AdapterList == null)
                return;
            if (Selected)
            {
                AdapterList.SelectItem(-1);
            }
            else
            {
                AdapterList.SelectItem(Index);
            }
        }

        // protected virtual void OnDataSwitching(TData before, TData after) { }
        protected abstract void OnDataUpdate(TData data);
        /// <summary>
        /// 数据清理前调用
        /// </summary>
        protected virtual void OnClearData() { }
        protected virtual void OnSelected() { }
        protected virtual void OnDeselected() { }

        void IPoolObject.OnSpawned(object data)
        {

        }

        void IPoolObject.OnDespawned()
        {
            // OnClearData();
            Data = default(TData);
        }

        /// <summary>
        /// 发送event到adapterList中
        /// </summary>
        /// <param name="key"></param>
        public void InvokeItemEvent(string key)
        {
            this.AdapterList?.InvokeItemEvent(Index, key);
        }

    }
}