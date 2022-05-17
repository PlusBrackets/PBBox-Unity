/*--------------------------------------------------------
 *Copyright (c) 2022 PlusBrackets
 *@update: 2022.03.29
 *@author: PlusBrackets
 --------------------------------------------------------*/
using System.Collections.Generic;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.Text;

namespace PBBox
{

    /// <summary>
    /// 文件的异步保存读取
    /// </summary>
    public sealed class FileAccesser : SingleClass<FileAccesser>
    {
        public abstract class Accesser
        {
            internal abstract Task FileWriter<T>(string path, T data);

            internal abstract Task<T> FileReader<T>(string path);
        }
        #region private File Accesser
        private class _FA_Binary : Accesser
        {
            internal override async Task<T> FileReader<T>(string path)
            {
                T _data = default;
                await Task.Run(() =>
                {
                    using (FileStream fs = new FileStream(path, FileMode.Open))
                    {
                        BinaryFormatter bf = new BinaryFormatter();
                        _data = (T)bf.Deserialize(fs);
                    }
                });
                return _data;
            }

            internal override async Task FileWriter<T>(string path, T data)
            {
                await Task.Run(() =>
                {
                    using (FileStream fs = new FileStream(path, FileMode.Create))
                    {
                        BinaryFormatter bf = new BinaryFormatter();
                        bf.Serialize(fs, data);
                    }
                });
            }
        }

        private class _FA_Text : Accesser
        {
            private Encoding m_encoding = Encoding.UTF8;
            public Encoding encoding
            {
                get
                {
                    return m_encoding;
                }
                internal set
                {
                    m_encoding = value == default ? Encoding.UTF8 : value;
                }
            }

            internal override async Task<T> FileReader<T>(string path)
            {
                T _data = default;
                using (StreamReader sr = new StreamReader(path, encoding))
                {
                    string _temp = await sr.ReadToEndAsync();
                    _data = (T)(object)_temp;

                }
                return _data;
            }

            internal override async Task FileWriter<T>(string path, T data)
            {
                using (StreamWriter sw = new StreamWriter(path, false, encoding))
                {
                    await sw.WriteAsync(data as string);
                }
            }
        }
        #endregion

        private enum AccessType
        {
            ReadFile,
            WriteFile
        }

        /// <summary>
        /// await锁，保持对同一个文件同时只有一个读写操作在进行
        /// </summary>
        private Dictionary<string, SemaphoreSlim> m_Semaphroes;

        private _FA_Binary m_FA_Binary;
        private _FA_Text m_FA_Text;

        private FileAccesser()
        {
            m_Semaphroes = new Dictionary<string, SemaphoreSlim>();
            m_FA_Binary = new _FA_Binary();
            m_FA_Text = new _FA_Text();
        }

        protected override void OnDispose()
        {
            base.OnDispose();
        }

        #region private func

        private SemaphoreSlim GetSemaphroe(string filePath)
        {
            if (!m_Semaphroes.ContainsKey(filePath))
            {
                m_Semaphroes.Add(filePath, new SemaphoreSlim(1, 1));
            }
            return m_Semaphroes[filePath];
        }

        private async Task<FileLoadResult<T>> AccessFile<T>(string filePath, AccessType accessType, Accesser accesser, T data = default, Action<FileLoadResult<T>> callBack = null)
        {
            FileResultCode _code = FileResultCode.SUCCESS;
            string _message = null;
            try
            {
                await GetSemaphroe(filePath).WaitAsync();
                string _directory = filePath.Substring(0, filePath.LastIndexOf('/') + 1);
                if (!Directory.Exists(_directory))
                {
                    Directory.CreateDirectory(_directory);
                }
                switch (accessType)
                {
                    case AccessType.ReadFile:
                        data = await accesser.FileReader<T>(filePath);
                        break;
                    case AccessType.WriteFile:
                        await accesser.FileWriter(filePath, data);
                        break;
                }
            }
            catch (FileNotFoundException ex)
            {
                _message = ex.Message;
                _code = FileResultCode.FILE_NOT_FOUND;
            }
            catch (FileLoadException ex)
            {
                _message = ex.Message;
                _code = FileResultCode.FILE_CANT_LOAD;
            }
            catch (SerializationException ex)
            {
                _message = ex.Message;
                _code = FileResultCode.SERIALIZATION_FAILED;
            }
            catch (Exception ex)
            {
                _message = ex.Message;
                _code = FileResultCode.ERROR;
            }
            finally
            {
                GetSemaphroe(filePath).Release();
                if (_code != FileResultCode.SUCCESS)
                {
                    DebugUtils.Internal.LogError($"[ERROR_{_code.ToString()}]:{_message}\n\tpath:{filePath}");
                }
            }
            FileLoadResult<T> _result = new FileLoadResult<T> { code = _code, data = data, message = _message };
            if (callBack != null)
            {
                callBack.Invoke(_result);
            }
            return _result;
        }
        #endregion

        #region Access By Accesser
        /// <summary>
        /// 使用FileAccesser读取文件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filePath"></param>
        /// <param name="accesser"></param>
        /// <returns></returns>
        public static async Task<FileLoadResult<T>> LoadByAccesser<T>(string filePath, Accesser accesser)
        {
            return await Instance.AccessFile<T>(filePath, AccessType.ReadFile, accesser);
        }

        /// <summary>
        /// 使用FileAccesser保存文件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filePath"></param>
        /// <param name="accesser"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static async Task<FileSaveResult> SaveByAccesser<T>(string filePath, Accesser accesser, T data)
        {
            FileLoadResult<T> _result = await Instance.AccessFile(filePath, AccessType.WriteFile, accesser, data);
            return new FileSaveResult { code = _result.code, message = _result.message };
        }

        /// <summary>
        /// 使用FileAccesser读取文件，利用callBack返回结果
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filePath"></param>
        /// <param name="accesser"></param>
        /// <param name="callBack"></param>
        public static async void LoadByAccesser<T>(string filePath, Accesser accesser, Action<FileLoadResult<T>> callBack)
        {
            await Instance.AccessFile(filePath, AccessType.ReadFile, accesser, default, callBack);
        }

        /// <summary>
        /// 使用FileAccesser保存文件，利用callBack返回结果
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filePath"></param>
        /// <param name="data"></param>
        /// <param name="accesser"></param>
        /// <param name="callBack"></param>
        public static async void SaveByAccesser<T>(string filePath, Accesser accesser, T data, Action<FileSaveResult> callBack)
        {
            await Instance.AccessFile<T>(filePath, AccessType.WriteFile, accesser, data, (_result) =>
              {
                  if (callBack != null)
                      callBack.Invoke(new FileSaveResult { code = _result.code, message = _result.message });
              });
        }

        #endregion

        #region Access By Binary Type

        /// <summary>
        /// 读取序列化的数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static async Task<FileLoadResult<T>> LoadBinary<T>(string filePath)
        {
            return await LoadByAccesser<T>(filePath, Instance.m_FA_Binary);
        }

        /// <summary>
        /// 写入序列化的数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filePath"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static async Task<FileSaveResult> SaveBinary<T>(string filePath, T data)
        {
            return await SaveByAccesser(filePath, Instance.m_FA_Binary, data);
        }

        /// <summary>
        /// 读取序列化的数据，结果通过callBack返回
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filePath"></param>
        /// <param name="callBack"></param>
        public static void LoadBinary<T>(string filePath, Action<FileLoadResult<T>> callBack)
        {
            LoadByAccesser(filePath, Instance.m_FA_Binary, callBack);
        }

        /// <summary>
        /// 写入序列化的数据，结果通过callBack返回
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filePath"></param>
        /// <param name="data"></param>
        /// <param name="callBack"></param>
        public static void SaveBinary<T>(string filePath, T data, Action<FileSaveResult> callBack)
        {
            SaveByAccesser(filePath, Instance.m_FA_Binary, data, callBack);
        }
        #endregion
        
        #region Access By Text Type
        /// <summary>
        /// 读取字符串
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static async Task<FileLoadResult<string>> LoadText(string filePath, Encoding encoding = default)
        {
            _FA_Text _accesser = Instance.m_FA_Text;
            _accesser.encoding = encoding;
            return await LoadByAccesser<string>(filePath, _accesser);
        }

        /// <summary>
        /// 写入字符串
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static async Task<FileSaveResult> SaveText(string filePath, string data, Encoding encoding = default)
        {
            _FA_Text _accesser = Instance.m_FA_Text;
            _accesser.encoding = encoding;
            return await SaveByAccesser(filePath, _accesser, data);
        }

        /// <summary>
        /// 读取字符串，结果用callBack返回
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="callBack"></param>
        public static void LoadText(string filePath, Action<FileLoadResult<string>> callBack, Encoding encoding = default)
        {
            _FA_Text _accesser = Instance.m_FA_Text;
            _accesser.encoding = encoding;
            LoadByAccesser(filePath, _accesser, callBack);
        }

        /// <summary>
        /// 写入字符串，结果用callBack返回
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="data"></param>
        /// <param name="callBack"></param>
        public static void SaveText(string filePath, string data, Action<FileSaveResult> callBack, Encoding encoding = default)
        {
            _FA_Text _accesser = Instance.m_FA_Text;
            _accesser.encoding = encoding;
            SaveByAccesser(filePath, _accesser, data, callBack);
        }
        #endregion

        // #region Other
        // public static bool CheckFileExists(string path)
        // {
        //     return File.Exists(path);
        // }

        // public static bool CheckDirectoryExists(string path)
        // {
        //     return Directory.Exists(path);
        // }
        // #endregion
        
        #region File Thread Protection
        private Thread m_ProtectThread;
        private bool m_IsProtecting = false;

        //test
        public void StartProtectThread()
        {
            DebugUtils.Internal.Log("开始文件存取保护线程");
            if (m_ProtectThread == null)
            {
                m_ProtectThread = new Thread(() =>
                {
                    while (m_IsProtecting)
                    {
                        Thread.Sleep(500);
                    }
                });
                m_IsProtecting = true;
                m_ProtectThread.Start();
            }
            else
            {
                DebugUtils.Internal.LogWarning("保护线程已经存在了");
            }
        }

        //test
        public void FinishProtectThread()
        {
            DebugUtils.Internal.Log("结束文件存取保护线程");
            m_IsProtecting = false;
            m_ProtectThread = null;
        }
        #endregion
    }
}