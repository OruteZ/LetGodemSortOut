using UnityEngine;

namespace Utility
{
    public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T instance;

        public static T Instance
        {
            get
            {
                if (instance != null) return instance;

                instance = FindObjectsByType<T>(FindObjectsInactive.Include, FindObjectsSortMode.None)[0];
                if (FindObjectsByType<T>(FindObjectsInactive.Include, FindObjectsSortMode.None).Length > 1)
                {
                    Debug.LogError("[Singleton] Something went really wrong " +
                                   " - 동일한 싱글톤 오브젝트가 두 개 있습니다. ");
                    return instance;
                }
                if (instance != null)
                {
                    Debug.Log("[Singleton] Using instance already created: " +
                              instance.gameObject.name);
                    return instance;
                }
                            
                // Create in Resources/Singletons/
                GameObject singleton = Resources.Load<GameObject>
                    ("Singletons/" + typeof(T).Name);
                if (singleton == null)
                {
                    Debug.LogError("[Singleton] An instance of " + typeof(T) +
                                   " : There is no prefab in Resources/Singletons/");
                    return null;
                }

                instance = Instantiate(singleton).GetComponent<T>();
                if (instance == null)
                {
                    Debug.LogError("[Singleton] + " + typeof(T) +
                                   " : There is no Component in GameObject.");
                    return instance;
                }
                                    
                Debug.LogWarning("[Singleton] Created in Resources : " + 
                                 instance.gameObject.name);
                return instance;
            }
        }

        private void OnDestroy()
        {
            instance = null;
        }
        
        protected virtual void Awake()
        {
            if (instance == null)
            {
                instance = this as T;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }

}