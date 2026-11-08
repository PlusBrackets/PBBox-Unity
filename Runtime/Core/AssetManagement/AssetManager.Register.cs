/*--------------------------------------------------------
 *Copyright (c) 2016-2026 PlusBrackets
 *@update: 2026.11.07
 *@author: PlusBrackets
 --------------------------------------------------------*/

namespace PBBox.AssetManagement
{

    internal sealed partial class AssetManager
    {
        //TODO 后续应该用源生成其自动生成注册代码
        partial void RegisterLoaderFactories()
        {
            RegisterLoaderFactory((int)AssetLoadType.Addressable, key => new AddressableAssetLoader(key));
            RegisterLoaderFactory((int)AssetLoadType.Resources, key => new ResourcesAssetLoader(key));
        }

        partial void RegisterDefaultUnloadStrategy()
        {
            SetUnloadStrategy(PBBoxSettings.AssetManagerDefaultUnloadStrategy);
        }
    }
}