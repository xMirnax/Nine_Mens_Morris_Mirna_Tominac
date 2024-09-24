using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MonoBehaviourSingleton<T> : MonoBehaviour where T : class
{
    public static T Instance => s_Instance;
    private static T s_Instance = null;

    protected virtual void Awake()
    {
        if (Instance == null)
        {
            s_Instance = this as T;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
