using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolSimple : MonoBehaviour
{
    public GameObject prefabGameObject;
    public int pooledAmount;

    [HideInInspector] public List<GameObject> objects;

    void Awake()
    {
        for (int i = 0; i < pooledAmount; i++)
        {
            GameObject obj = (GameObject)Instantiate(prefabGameObject);
            obj.transform.parent = gameObject.transform;
            obj.SetActive(false);
            objects.Add(obj);
        }
    }
}


/* Use Pool
    
    [SerializeField]private ObjectPoolSimple _ObjectPool;

    private void Spawn() {
        for (int i = 0; i < _ObjectPool.objects.Count; i++) {
            if (!_ObjectPool.objects[i].activeInHierarchy) {
                _ObjectPool.objects[i].transform.position = new Vector3(0,0,0);
                _ObjectPool.objects[i].transform.rotation = Quaternion.Euler(0, 0, 0);
                _ObjectPool.objects[i].SetActive(true);
                break;
            }
        }
    }
*/
