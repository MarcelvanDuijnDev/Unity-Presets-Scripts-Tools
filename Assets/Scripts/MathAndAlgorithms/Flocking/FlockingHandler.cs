using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlockingHandler : MonoBehaviour {
    
    [Header("Settings")]
    [SerializeField] private GameObject _UnitPrefab = null;
    [SerializeField] private int _SetUnitAmount = 200;
    [SerializeField] private int _FlockingArea = 20;

    public static int _FlockArea = 20;
    static int _UnitAmount = 0;
    public static GameObject[] _AllUnits = new GameObject[_UnitAmount];

    public static Vector3 _GoalPos = Vector3.zero;

	void Start () 
    {
        _FlockArea = _FlockingArea;
        _UnitAmount = _SetUnitAmount;
        _AllUnits = new GameObject[_UnitAmount];

        for (int i = 0; i < _UnitAmount; i++)
        {
            Vector3 pos = new Vector3(  Random.Range(-_FlockArea, _FlockArea),
                                        Random.Range(-_FlockArea, _FlockArea),
                                        Random.Range(-_FlockArea, _FlockArea));
            _AllUnits[i] = (GameObject)Instantiate(_UnitPrefab, pos, Quaternion.identity);
        }
	}

    void Update()
    {
        if (Random.Range(0, 10000) < 50)
        {
            _GoalPos = new Vector3(  Random.Range(-_FlockArea, _FlockArea),
                                    Random.Range(-_FlockArea, _FlockArea),
                                    Random.Range(-_FlockArea, _FlockArea));
        }
    }
}
