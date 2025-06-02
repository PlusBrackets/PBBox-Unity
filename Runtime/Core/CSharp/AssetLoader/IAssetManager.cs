/*--------------------------------------------------------
 *Copyright (c) 2016-2025 PlusBrackets
 *@update: 2025.06.02
 *@author: PlusBrackets
 --------------------------------------------------------*/
namespace PBBox
{

    public interface IAssetManager : ISingleton<IAssetManager>
    {
        TLoader GetLoader<TLoader>(string key) where TLoader : IAssetLoader;
        IAssetLoader GetLoader(string key, int loaderType = 0);
    }
}