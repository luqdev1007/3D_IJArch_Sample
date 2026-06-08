using UnityEngine;

namespace Assets._Project.Develop.Runtime.Utilities.AssetsManagment
{
    public class ResourcesAssetsLoader
    {
        public T Load<T>(string resourcePath) where T : Object
            => Resources.Load<T>(resourcePath);
    }
}