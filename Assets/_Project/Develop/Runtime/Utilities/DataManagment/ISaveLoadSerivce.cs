using System;
using System.Collections;

namespace Assets._Project.Develop.Runtime.Utilities.DataManagment
{
    public interface ISaveLoadSerivce
    {
        IEnumerator Load<TData>(Action<TData> onLoad) where TData : ISaveData;
        IEnumerator Save<TData>(TData data) where TData : ISaveData;
        IEnumerator Remove<TData>() where TData : ISaveData;
        IEnumerator Exists<TData>(Action<bool> onExistsResult) where TData : ISaveData;
    }
}
