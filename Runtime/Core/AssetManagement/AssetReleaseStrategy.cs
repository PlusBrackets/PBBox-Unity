/*--------------------------------------------------------
 *Copyright (c) 2016-2026 PlusBrackets
 *@update: 2026.11.06
 *@author: PlusBrackets
 --------------------------------------------------------*/

namespace PBBox.AssetManagement
{
    public struct AssetReleaseStrategy
    {
        /// <summary>
        /// 是否手动释放资源，如果为true，则需要手动调用ReleaseAssetHandle来释放资源，否则将根据设定的策略自动释放资源。
        /// </summary>
        public bool isManual;
        /// <summary>
        /// 每隔多少秒尝试释放一次资源
        /// </summary>
        public float? intervalSec;
        /// <summary>
        /// 延迟多少秒后释放资源，从引用计数为0开始计时，重新引用时会重置计时器。
        /// </summary>
        public float? delaySec;
        /// <summary>
        /// 当内存使用量超过多少MB时，尝试释放资源
        /// </summary>
        public int? lowMemoryThreshold;
        /// <summary>
        /// 当内存使用量超过多少MB时，才开始尝试自动释放资源，低于该值时不会自动释放资源。
        /// </summary>
        public int? minMemoryThreshold;
        /// <summary>
        /// 每次释放资源的最大数量，超过该数量的资源将会在下一帧再释放。
        /// </summary>
        public int? maxReleaseCountPerFrame;


        public static AssetReleaseStrategy Default => new AssetReleaseStrategy
        {
            isManual = false,
            intervalSec = 10f,
            delaySec = 120f,
            lowMemoryThreshold = 1024 * 16,
            minMemoryThreshold = 1024 * 2,
            maxReleaseCountPerFrame = 32
        };

        /// <summary>
        /// 手动释放资源策略，所有资源都不会自动释放，需要手动调用释放方法来释放资源。
        /// </summary>
        public static AssetReleaseStrategy Manual => new AssetReleaseStrategy
        {
            isManual = true,
            intervalSec = null,
            delaySec = null,
            lowMemoryThreshold = null,
            minMemoryThreshold = null,
            maxReleaseCountPerFrame = null
        };

        public static AssetReleaseStrategy Immediate => new AssetReleaseStrategy
        {
            isManual = false,
            intervalSec = 0f,
            delaySec = 0f,
            lowMemoryThreshold = 0,
            minMemoryThreshold = 0,
            maxReleaseCountPerFrame = null
        };
    }
}