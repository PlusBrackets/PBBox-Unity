/*--------------------------------------------------------
 *Copyright (c) 2022 PlusBrackets
 *@update: 2022.04.13
 *@author: PlusBrackets
 --------------------------------------------------------*/
using System.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace PBBox.Persistence
{
    /// <summary>
    /// 数据存档读档工具
    /// </summary>
    public sealed class DataPersistence : SingleClass<DataPersistence>
    {
        private Dictionary<string, IPersistentContainer> m_ContainerDict;

        ///// <summary>
        ///// 注册存档容器时触发
        ///// </summary>
        //public Action<string> onRegisterContainer;

        /// <summary>
        /// 设置存档文件的位置，默认为PersistentDataPath/SaveData/;
        /// </summary>
        public string saveDataPath { get; set; }

        /// <summary>
        /// 保存操作的队列数
        /// </summary>
        public int saveQueueCount { get; private set; } = 0;

        /// <summary>
        /// 读取操作的对列数
        /// </summary>
        public int loadQueueCount { get; private set; } = 0;

        /// <summary>
        /// 是否正在存取中
        /// </summary>
        public bool isProccessing
        {
            get
            {
                return isSaving || isLoading;
            }
        }

        /// <summary>
        /// 是否正在保存中
        /// </summary>
        public bool isSaving
        {
            get
            {
                return saveQueueCount != 0;
            }
        }

        /// <summary>
        /// 是否正在读取中
        /// </summary>
        public bool isLoading
        {
            get
            {
                return loadQueueCount != 0;
            }
        }

        private SemaphoreSlim m_Lock = new SemaphoreSlim(1, 1);
        ///// <summary>
        ///// 最大存取线程数
        ///// </summary>
        //public int maxAccessThread { get; set; } = 2;

        #region Initialization

        private DataPersistence()
        {
            saveDataPath = Application.persistentDataPath + "/SaveData/";
            m_ContainerDict = new Dictionary<string, IPersistentContainer>();
        }

        /// <summary>
        /// 注册存档容器，在save或者load前应该要先注册容器
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dataID"></param>
        /// <param name="fileName"></param>
        public DataContainer<T> RegisterContainer<T>(string dataID, string fileName) where T : IPersistentData, new()
        {
            if (m_ContainerDict.ContainsKey(dataID))
            {
                DebugUtils.Internal.LogWarning($"[PBBox_Persistence]已经注册过该容器[id:{dataID}]");
                return null;
            }
            //if (onRegisterContainer != null)
            //{
            //    onRegisterContainer.Invoke(dataID);
            //}
            DataContainer<T> _temp = new DataContainer<T>(dataID, fileName);
            m_ContainerDict.Add(dataID, _temp);
            return _temp;
        }

        /// <summary>
        /// 注销存档容器
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dataID"></param>
        public static void UnregisterContainer(string dataID)
        {
            if (!HasInstance)
                return;
            if (Instance.m_ContainerDict.ContainsKey(dataID))
            {
                Instance.m_ContainerDict[dataID].CallUnregister();
                Instance.m_ContainerDict.Remove(dataID);
            }
        }

        /// <summary>
        /// 注销所有存档容器
        /// </summary>
        public static void UnregisterAllContainer(){
            if(!HasInstance)
                return;
            string[] keys = Instance.m_ContainerDict.Keys.ToArray();
            foreach(string key in keys){
                Instance.m_ContainerDict[key].CallUnregister();
                Instance.m_ContainerDict.Remove(key);
            }
        }

        /// <summary>
        /// 获取存档容器
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dataID"></param>
        /// <returns></returns>
        public DataContainer<T> GetDataContainer<T>(string dataID) where T : IPersistentData, new()
        {
            IPersistentContainer _container = GetDataContainer(dataID);
            if (_container != null)
                return _container as DataContainer<T>;
            return null;
        }

        /// <summary>
        /// 获取存档容器
        /// </summary>
        /// <param name="dataID"></param>
        /// <returns></returns>
        public IPersistentContainer GetDataContainer(string dataID)
        {
            IPersistentContainer _container = null;
            if (m_ContainerDict.TryGetValue(dataID, out _container))
            {
                return _container;
            }
            else
            {
                DebugUtils.Internal.LogWarning($"[PBBox_Persistence]数据内不存在该容器[id:{dataID}]");
                return null;
            }

        }

        /// <summary>
        /// 获取存档数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dataID"></param>
        /// <returns></returns>
        public T GetData<T>(string dataID) where T : IPersistentData, new()
        {
            DataContainer<T> _c = GetDataContainer<T>(dataID);
            if (_c != null)
                return _c.data;
            else
                return default(T);
        }

        #endregion
        
        /// <summary>
        /// 保存数据到本地
        /// </summary>
        /// <param name="dataID"></param>
        /// <param name="callBack"></param>
        /// <returns></returns>
        public async Task<FileResultCode> Save(string dataID, Action<string, FileResultCode> callBack = null)
        {
            saveQueueCount++;
            await m_Lock.WaitAsync();
            FileResultCode _code = FileResultCode.ERROR;
            IPersistentContainer _container = GetDataContainer(dataID);
            if (_container != null)
            {
                _code = await _container.Save();
            }
            m_Lock.Release();
            saveQueueCount--;
            if (callBack != null)
                callBack.Invoke(dataID, _code);
            return _code;
        }

        /// <summary>
        /// 保存多个数据到本地
        /// </summary>
        /// <param name="dataIDs"></param>
        /// <param name="callBack"></param>
        /// <returns></returns>
        public async Task<Dictionary<string,FileResultCode>> Save(string[] dataIDs, Action<Dictionary<string,FileResultCode>> callBack = null)
        {
            Dictionary<string, FileResultCode> _result = new Dictionary<string, FileResultCode>();
            for (int i = 0; i < dataIDs.Length; i++)
            {
                FileResultCode _code = await Save(dataIDs[i], null);
                _result.Add(dataIDs[i], _code);
            }
            if (callBack != null)
            {
                callBack.Invoke(_result);
            }
            return _result;
        }

        /// <summary>
        /// 保存多个数据到本地
        /// </summary>
        /// <param name="callBack"></param>
        /// <param name="dataIDs"></param>
        public async void Save(Action<Dictionary<string, FileResultCode>> callBack,params string[] dataIDs)
        {
            await Save(dataIDs, callBack);
        }

        /// <summary>
        /// 从本地读取数据
        /// </summary>
        /// <param name="dataID"></param>
        /// <param name="callBack"></param>
        /// <returns></returns>
        public async Task<FileResultCode> Load(string dataID, Action<string, FileResultCode> callBack = null)
        {
            loadQueueCount++;
            await m_Lock.WaitAsync();
            FileResultCode _code = FileResultCode.ERROR;
            IPersistentContainer _container = GetDataContainer(dataID);
            if (_container != null)
            {
                _code = await _container.Load();
            }
            m_Lock.Release();
            loadQueueCount--;
            if (callBack != null)
                callBack.Invoke(dataID, _code);
            return _code;
        }

        /// <summary>
        /// 从本地读取多个数据
        /// </summary>
        /// <param name="dataIDs"></param>
        /// <param name="callBack"></param>
        /// <returns></returns>
        public async Task<Dictionary<string,FileResultCode>> Load(string[] dataIDs, Action<Dictionary<string, FileResultCode>> callBack = null)
        {
            Dictionary<string, FileResultCode> _result = new Dictionary<string, FileResultCode>();
            for (int i = 0; i < dataIDs.Length; i++)
            {
                FileResultCode _code = await Load(dataIDs[i], null);
                _result.Add(dataIDs[i], _code);
            }
            if (callBack != null)
            {
                callBack.Invoke(_result);
            }
            return _result;
        }

        /// <summary>
        /// 从本地读取多个数据
        /// </summary>
        /// <param name="callBack"></param>
        /// <param name="dataIDs"></param>
        public async void Load(Action<Dictionary<string, FileResultCode>> callBack,params string[] dataIDs)
        {
            await Load(dataIDs, callBack);
        }
    }
}