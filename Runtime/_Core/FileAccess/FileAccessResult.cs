/*--------------------------------------------------------
 *Copyright (c) 2022 PlusBrackets
 *@update: 2022.03.29
 *@author: PlusBrackets
 --------------------------------------------------------*/
namespace PBBox
{
    /// <summary>
    /// 读取操作的结果
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public struct FileLoadResult<T>
    {
        public FileResultCode code;
        public T data;
        public string message;
    }

    /// <summary>
    /// 写入操作的结果
    /// </summary>
    public struct FileSaveResult
    {
        public FileResultCode code;
        public string message;
    }

    public enum FileResultCode
    {
        /// <summary>
        /// 错误
        /// </summary>
        ERROR = -1,
        /// <summary>
        /// 成功
        /// </summary>
        SUCCESS,
        /// <summary>
        /// 文件不存在
        /// </summary>
        FILE_NOT_FOUND,
        /// <summary>
        /// 文件无法读取
        /// </summary>
        FILE_CANT_LOAD,
        /// <summary>
        /// 序列化失败
        /// </summary>
        SERIALIZATION_FAILED,
    }
}