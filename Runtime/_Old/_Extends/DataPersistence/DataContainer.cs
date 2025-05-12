/*--------------------------------------------------------
 *Copyright (c) 2022 PlusBrackets
 *@update: 2022.04.13
 *@author: PlusBrackets
 --------------------------------------------------------*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using PBBox.Utils;

namespace PBBox.Persistence
{

    public interface IPersistentContainer
    {
        string dataID { get; }

        string fileName { get; set; }

        bool hasFile { get; }

        IPersistentData GetData();
        Type GetDataType();

        Task<FileResultCode> Save();
        Task<FileResultCode> Load();

        void CallUnregister();
    }

    /// <summary>
    /// 持续化数据的容器,使用JsonUtility存取
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class DataContainer<T> : IPersistentContainer where T : IPersistentData, new()
    {
        /// <summary>
        /// 绑定的数据
        /// </summary>
        public T data { get; internal set; }

        /// <summary>
        /// 是否是经过读取的数据
        /// </summary>
        public bool isLoaded { get; private set; } = false;

        /// <summary>
        /// 数据id
        /// </summary>
        public string dataID { get; private set; }

        /// <summary>
        /// 文件名
        /// </summary>
        public string fileName { get; set; }

        /// <summary>
        /// 当前路径是否有文件
        /// </summary>
        public bool hasFile
        {
            get { return File.Exists(filePath); }
        }
        /// <summary>
        /// 完整文件路径
        /// </summary>
        public string filePath
        {
            get
            {
                return DataPersistence.Instance.saveDataPath + fileName;
            }
        }

        /// <summary>
        /// 是否保留上次的存档数据为 [fileName].backup
        /// </summary>
        public bool isBackUpFile = false;

        internal DataContainer(string id, string fileName)
        {
            dataID = id;
            this.fileName = fileName;
            data = new T();
        }

        public class PersistenceEvent : UnityEvent<DataContainer<T>> { }
        /// <summary>
        /// 当被注销时
        /// </summary>
        public PersistenceEvent onUnregister = new PersistenceEvent();
        /// <summary>
        /// 当要保存时
        /// </summary>
        public PersistenceEvent onBeforeSave = new PersistenceEvent();
        /// <summary>
        /// 当保存之后
        /// </summary>
        public PersistenceEvent onAfterSave = new PersistenceEvent();
        /// <summary>
        /// 当要被读取时
        /// </summary>
        public PersistenceEvent onBeforeLoad = new PersistenceEvent();
        /// <summary>
        /// 当被读取之后
        /// </summary>
        public PersistenceEvent onAfterLoad = new PersistenceEvent();

        /// <summary>
        /// 获得数据
        /// </summary>
        /// <returns></returns>
        public IPersistentData GetData()
        {
            return data;
        }

        /// <summary>
        /// 数据的类型
        /// </summary>
        /// <returns></returns>
        public Type GetDataType()
        {
            return typeof(T);
        }

        void IPersistentContainer.CallUnregister()
        {
            onUnregister.Invoke(this);
        }

        async Task<FileResultCode> IPersistentContainer.Save()
        {
            onBeforeSave.Invoke(this);
            if (data != null)
                data.BeforeSavePersistent(dataID);
            string _tempPath = filePath + ".temp";

            string json = JsonUtility.ToJson(data);
            FileSaveResult _result = await FileAccesser.SaveText(_tempPath, json);

            if (_result.code == FileResultCode.SUCCESS)
            {
                try
                {
                    if (File.Exists(filePath))
                    {
                        if (isBackUpFile)
                        {
                            string _backupPath = filePath + ".backup";
                            if (File.Exists(_backupPath))
                                File.Delete(_backupPath);
                            File.Move(filePath, _backupPath);
                        }
                        else
                        {
                            File.Delete(filePath);
                        }
                    }
                    File.Move(_tempPath, filePath);
                }
                catch (Exception e)
                {
                    DebugUtils.Internal.LogError(e);
                }
            }
            if (data != null)
                data.AfterSavePersistent(dataID, _result.code);
            onAfterSave.Invoke(this);
            return _result.code;
        }

        async Task<FileResultCode> IPersistentContainer.Load()
        {
            onBeforeLoad.Invoke(this);
            if (data != null)
                data.BeforeLoadPersistent(dataID);
            FileLoadResult<string> _result = await FileAccesser.LoadText(filePath);
            if (_result.code == FileResultCode.SUCCESS)
            {
                JsonUtility.FromJsonOverwrite(_result.data, data);
            }
            isLoaded = _result.code == FileResultCode.SUCCESS;
            if (data != null)
                data.AfterLoadPersistent(dataID, _result.code);
            onAfterLoad.Invoke(this);
            return _result.code;
        }

    }
}