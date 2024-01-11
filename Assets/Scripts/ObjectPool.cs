using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ObjectToPool
{
    public GameObject prefab;
    public int count;
}

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool Instance { get; private set; }

    [SerializeField] private List<ObjectToPool> prefabs = new List<ObjectToPool>();
    private List<GameObject> pooledObjects = new List<GameObject>();

    
    private void Awake()
    {
        if (!Instance)
        {
            Instance = this;
            //DontDestroyOnLoad(this);
        }
        else Destroy(gameObject);

        foreach (var item in prefabs)
        {
            for (int i = 0; i < item.count; i++)
            {
                GameObject obj = Instantiate(item.prefab);
                obj.transform.parent = transform;
                obj.name = item.prefab.name;
                obj.SetActive(false);
                pooledObjects.Add(obj);
            }
        }
    }

    public GameObject GetPooledObject(GameObject prefab)
    {
        for (int i = 0; i < pooledObjects.Count; i++)
        {
            if (!pooledObjects[i].activeInHierarchy && pooledObjects[i].gameObject.name == prefab.name)
            {
                return pooledObjects[i];
            }
        }

        return null;
    }
}
