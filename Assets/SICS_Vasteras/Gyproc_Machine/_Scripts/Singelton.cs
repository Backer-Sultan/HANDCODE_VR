using UnityEngine;
using System.Collections;

public class Singelton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;
    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<T>();
                if (instance == null)
                {
                    GameObject obj = new GameObject();
                    obj.name = typeof(T).Name;
                    instance = obj.AddComponent<T>();
                }
            }
            return instance;
        }
    }

    public virtual void Awake()
    {
        if (instance == null)
        {
            instance = this as T;
            /* I don't want Main.cs to stay in memory when I reload the scene, as I don't 
             * transport it over multiple scenes.
             * Being in memory when reloading the scene creating a problem of missing references
             * (as they are destroyed by reloading)
             * So in this project, we don't need DontDestroyOnLoad()
             * but in general this is very important to use in singelton, as it's behind the idea
             * of transporting data over scenes.
             */
            //DontDestroyOnLoad(this.gameObject);
        }
        else
            Destroy(gameObject);
    }

}
