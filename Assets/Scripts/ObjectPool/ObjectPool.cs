using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    [SerializeField] private ObjectPool_Pool[] _ObjectPools = null;
    private List<Transform> _Parents = new List<Transform>();

    public static ObjectPool POOL;

    private void Awake()
    {
        POOL = this;

        for (int i = 0; i < _ObjectPools.Length; i++)
        {
            //Create parent
            GameObject poolparent = new GameObject();
            Destroy(poolparent.GetComponent<MeshRenderer>());
            Destroy(poolparent.GetComponent<BoxCollider>());

            //Set parent
            poolparent.transform.parent = transform;
            poolparent.transform.name = "Pool_" + _ObjectPools[i]._Name;
            _Parents.Add(poolparent.transform);

            //Create objects
            for (int o = 0; o < _ObjectPools[i]._Amount; o++)
            {
                GameObject obj = (GameObject)Instantiate(_ObjectPools[i]._Prefab);
                obj.transform.parent = poolparent.transform;
                obj.transform.position = new Vector2(9999, 9999);
                obj.SetActive(false);
                _ObjectPools[i]._Objects.Add(obj);
            }
        }
    }

    //GetObject
    public GameObject GetObject(string objname)
    {
        int id = FindObjectPoolID(objname, false);
        return GetObject(id, true);
    }
    public GameObject GetObject(GameObject obj)
    {
        int id = FindObjectPoolID(obj);
        return GetObject(id, true);
    }
    public GameObject GetObjectPrefabName(string prefabname)
    {
        int id = FindObjectPoolID(prefabname, true);
        return GetObject(id, true);
    }

    //GetObject/setactive
    public GameObject GetObject(string objname, bool setactive)
    {
        int id = FindObjectPoolID(objname, false);
        return GetObject(id, setactive);
    }
    public GameObject GetObject(GameObject obj, bool setactive)
    {
        int id = FindObjectPoolID(obj);
        return GetObject(id, setactive);
    }
    public GameObject GetObjectPrefabName(string prefabname, bool setactive)
    {
        int id = FindObjectPoolID(prefabname, true);
        return GetObject(id, setactive);
    }

    public GameObject GetObject(int id, bool setactive)
    {
        GameObject freeObject = null;

        for (int i = 0; i < _ObjectPools[id]._Objects.Count; i++)
        {
            if (!_ObjectPools[id]._Objects[i].activeInHierarchy)
            {
                _ObjectPools[id]._Objects[i].transform.position = new Vector3(999, 999, 999);
                _ObjectPools[id]._Objects[i].SetActive(setactive);
                freeObject = _ObjectPools[id]._Objects[i];
                return freeObject;
            }
        }

        freeObject = (GameObject)Instantiate(_ObjectPools[id]._Prefab, new Vector3(999, 999, 999), Quaternion.identity);
        freeObject.transform.parent = _Parents[id];
        freeObject.SetActive(setactive);
        _ObjectPools[id]._Objects.Add(freeObject);
        return freeObject;
    }

    public List<GameObject> GetAllObjects(GameObject objtype)
    {
        int id = FindObjectPoolID(objtype);
        return _ObjectPools[id]._Objects;
    }

    private int FindObjectPoolID(GameObject obj)
    {
        int id = 0;
        for (int i = 0; i < _ObjectPools.Length; i++)
        {
            if (obj == _ObjectPools[i]._Prefab)
            {
                id = i;
            }
        }
        return id;
    }
    private int FindObjectPoolID(string objname, bool isprefab)
    {
        for (int i = 0; i < _ObjectPools.Length; i++)
        {
            if (isprefab)
            {
                if (objname == _ObjectPools[i]._Prefab.name)
                    return i;
            }
            else if (objname == _ObjectPools[i]._Name)
                return i;
        }
        Debug.Log(objname + " Not Found");
        return 0;
    }
}

[System.Serializable]
public class ObjectPool_Pool
{
    public string _Name;
    public GameObject _Prefab;
    public int _Amount;
    [HideInInspector] public List<GameObject> _Objects;
}