using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour 
{
    public static T Instance => _instance != null ? _instance : _instanceOnEditor; // _instance ?? _instanceOnEditor; <- this will not work in Unity!
    private static T _instance;
    private static T _instanceOnEditor => FindObjectOfType<T>();
    protected virtual void Awake() 
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this);
        }
        else
        {
            _instance = this as T;
        }
    }
}

public abstract class SingletonPersistent<T> : Singleton<T> where T : MonoBehaviour
{
    protected override void Awake() 
    {
        if(Instance != null) 
        {
            // delete duplicates
            Destroy(gameObject);
            return;
        }
        else
        {
            DontDestroyOnLoad(gameObject);
            base.Awake();
        }
    }
}

