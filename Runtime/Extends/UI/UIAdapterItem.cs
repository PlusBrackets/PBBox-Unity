using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PBBox.UI
{
    public abstract class UIAdapterItem<Data> : MonoBehaviour, IPoolObject
    {
        SimplePool IPoolObject.pool { get; set; }
        public Data data { get; protected set; }
        public UIAdapterList<Data> adapterList { get; private set; }
        public int index { get; private set; } = 0;
        public bool selected { get; internal set; } = false;

        public virtual bool IsSelectable()
        {
            return false;
        }

        public virtual void SetData(Data data, UIAdapterList<Data> list, int index)
        {
            if ((object)this.data!= (object)data)
            {
                OnDataSwitching(this.data, data);
            }
            this.data = data;
            this.adapterList = list;
            this.index = index;
            OnDataUpdate(data);
        }

        protected virtual void OnDataSwitching(Data before, Data after) { }

        protected abstract void OnDataUpdate(Data data);

        internal virtual void OnSelected() { }

        internal virtual void OnDeselect() { }

        void IPoolObject.OnSpawned(object[] datas)
        {
         
        }

        void IPoolObject.OnDespawned()
        {
            data = default(Data);
        }
    }
}