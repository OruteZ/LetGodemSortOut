using UnityEngine;

namespace Utility
{
    public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T _instance;

        public static T Instance
        {
            get
            {
                if (_instance != null) return _instance;

                _instance = FindObjectsByType<T>(FindObjectsInactive.Include, FindObjectsSortMode.None)[0];
                if (FindObjectsByType<T>(FindObjectsInactive.Include, FindObjectsSortMode.None).Length > 1)
                {
                    Debug.LogError("[Singleton] Something went really wrong " +
                                   " - 동일한 싱글톤 오브젝트가 두 개 있습니다. ");
                    return _instance;
                }
                if (_instance != null)
                {
                    Debug.Log("[Singleton] Using instance already created: " +
                              _instance.gameObject.name);
                    return _instance;
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

                _instance = Instantiate(singleton).GetComponent<T>();
                if (_instance == null)
                {
                    Debug.LogError("[Singleton] + " + typeof(T) +
                                   " : There is no Component in GameObject.");
                    return _instance;
                }
                                    
                Debug.LogWarning("[Singleton] Created in Resources : " + 
                                 _instance.gameObject.name);
                return _instance;
            }
        }

        private void OnDestroy()
        {
            _instance = null;
        }
        
        protected virtual void Awake()
        {
            if (_instance == null)
            {
                _instance = this as T;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }

}