/*--------------------------------------------------------
 *Copyright (c) 2022 PlusBrackets
 *@update: 2022.04.13
 *@author: PlusBrackets
 --------------------------------------------------------*/
namespace PBBox.Persistence
{

    /// <summary>
    /// 存放持续性数据的类
    /// </summary>
    // [Serializable]
    public interface IPersistentData
    {
        /// <summary>
        /// 准备保存的时候会被调用
        /// </summary>
        /// <param name="dataID"></param>
        void BeforeSavePersistent(string dataID);
        /// <summary>
        /// 保存后调用
        /// </summary>
        /// <param name="dataID"></param>
        /// <param name="resultCode"></param>
        void AfterSavePersistent(string dataID, FileResultCode resultCode);
        /// <summary>
        /// 准备读取的时候会被调用
        /// </summary>
        /// <param name="dataID"></param>
        void BeforeLoadPersistent(string dataID);
        /// <summary>
        /// 读取完成时调用
        /// </summary>
        /// <param name="dataID"></param>
        /// <param name="resultCode"></param>
        void AfterLoadPersistent(string dataID, FileResultCode resultCode);
        
    }

    public class BasePersistentData:IPersistentData{
        
        /// <summary>
        /// 准备保存的时候会被调用
        /// </summary>
        /// <param name="dataID"></param>
        public virtual void BeforeSavePersistent(string dataID){

        }

        /// <summary>
        /// 保存后调用
        /// </summary>
        /// <param name="dataID"></param>
        /// <param name="resultCode"></param>
        public virtual void AfterSavePersistent(string dataID, FileResultCode resultCode){

        }

        /// <summary>
        /// 准备读取的时候会被调用
        /// </summary>
        /// <param name="dataID"></param>
        public virtual void BeforeLoadPersistent(string dataID){

        }

        /// <summary>
        /// 读取完成时调用
        /// </summary>
        /// <param name="dataID"></param>
        /// <param name="resultCode"></param>
        public virtual void AfterLoadPersistent(string dataID, FileResultCode resultCode){

        }
    }
}