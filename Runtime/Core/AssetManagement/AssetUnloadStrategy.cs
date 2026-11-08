/*--------------------------------------------------------
 *Copyright (c) 2016-2026 PlusBrackets
 *@update: 2026.11.06
 *@author: PlusBrackets
 --------------------------------------------------------*/

namespace PBBox.AssetManagement
{
    [System.Serializable]
    public struct AssetUnloadStrategy
    {
        /// <summary>
        /// 是否手动释放资源，如果为true，则需要手动调用ReleaseAssetHandle来释放资源，否则将根据设定的策略自动释放资源。
        /// </summary>
        public bool isManual;
        /// <summary>
        /// 每隔多少毫秒尝试释放一次资源，小于等于0或null表示不尝试。
        /// </summary>
        public uint intervalMs;
        /// <summary>
        /// 延迟多少毫秒后释放资源，从引用计数为0开始计时，重新引用时会重置计时器。小于等于0或null表示不延迟。
        /// </summary>
        public uint delayMs;

        //TODO 考虑内存使用量相关的策略，目前暂时注释掉
        ///// <summary>
        ///// 当内存使用量超过多少MB时，尝试强制释放资源，小于等于0则不限制。
        ///// </summary>
        //public uint lowMemoryThreshold;
        ///// <summary>
        ///// 当内存使用量超过多少MB时，才开始尝试自动释放资源，低于该值时不会自动释放资源。小于等于0表示不限制。
        ///// </summary>
        //public uint minMemoryThreshold;

        /// <summary>
        /// 每次释放资源的最大数量，超过该数量的资源将会在下一帧再释放。小于等于0则无限
        /// </summary>
        public uint maxUnloadCountPerFrame;

        public bool IsImmediate => !isManual && intervalMs == 0 && delayMs == 0;

        /// <summary>
        /// 默认的资源释放策略。
        /// </summary>
        public static AssetUnloadStrategy DefaultAuto => new AssetUnloadStrategy
        {
            isManual = false,
            intervalMs = 10 * 1000,
            delayMs = 120 * 1000,
            //lowMemoryThreshold = 0,
            //minMemoryThreshold = 1024 * 2,
            maxUnloadCountPerFrame = 32
        };

        /// <summary>
        /// 立即释放资源策略，所有资源在引用计数为0时立即释放，不会有任何延迟。
        /// </summary>
        public static AssetUnloadStrategy Immediate => new AssetUnloadStrategy
        {
            isManual = false,
            intervalMs = 0,
            delayMs = 0,
            //lowMemoryThreshold = 0,
            //minMemoryThreshold = 0,
            maxUnloadCountPerFrame = 0
        };


        /// <summary>
        /// 手动释放资源策略，所有资源都不会自动释放，需要手动调用释放方法来释放资源。
        /// </summary>
        public static AssetUnloadStrategy Manual => new AssetUnloadStrategy
        {
            isManual = true,
            intervalMs = 0,
            delayMs = 0,
            //lowMemoryThreshold = 0,
            //minMemoryThreshold = 0,
            maxUnloadCountPerFrame = 0
        };
    }
}