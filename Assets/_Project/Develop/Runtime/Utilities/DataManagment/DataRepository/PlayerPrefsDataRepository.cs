using System;
using System.Collections;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Utilities.DataManagment.DataRepository
{
    public class PlayerPrefsDataRepository : IDataRepository
    {
        public IEnumerator Exists(string key, Action<bool> onExistsResult)
        {
            bool exists = PlayerPrefs.HasKey(key);

            onExistsResult?.Invoke(exists);

            yield break;
        }

        public IEnumerator Read(string key, Action<string> onRead)
        {
            string text = PlayerPrefs.GetString(key);

            onRead?.Invoke(text);

            yield break;
        }

        public IEnumerator Remove(string key)
        {
            PlayerPrefs.DeleteKey(key);

            yield break;
        }

        public IEnumerator Write(string key, string serializedData)
        {
            PlayerPrefs.SetString(key, serializedData);

            yield break;
        }
    }
}
