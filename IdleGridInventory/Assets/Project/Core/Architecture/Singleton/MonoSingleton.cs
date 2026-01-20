using UnityEngine;

namespace OmniGameTemplate.Core
{
    public class MonoSingleton<T> : MonoBehaviour
     where T : Component
    {
        private static T instance;

        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    var objects = FindObjectsByType<T>(FindObjectsInactive.Include, FindObjectsSortMode.None);

                    if (objects.Length > 1)
                    {
                        Debug.LogError($"Singleton class can not have more than one. : {typeof(T).Name}");
                    }
                    else if (objects.Length == 1)
                    {
                        instance = objects[0];
                    }
                }
                return instance;
            }
        }
    }
}