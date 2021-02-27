using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LangtonsAnt : MonoBehaviour {

    [Header("Info")]
    [SerializeField] private int _Itterations;

    //Rulesets not working yet
    [Header("Settings")]
    private string _RuleSet = "L";
    [SerializeField] private float _UpdatesInSeconds = 0;
    [SerializeField] private Transform _PrefabObj_Left;
    [SerializeField] private Transform _PrefabObj_Right;

    private bool _Collision;
    private Vector3 _PrevPos;
    private float _Timer;

    private int _Step;
    private int _MaxStep;

    private void Start()
    {
        _MaxStep = _RuleSet.Length;
    }

    void FixedUpdate () 
    {
        _Timer += 1 * Time.deltaTime;

        if (_Timer >= _UpdatesInSeconds)
        {
            _Itterations++;
            _PrevPos = transform.position;
            CheckMove();
            _Timer = 0;
            transform.Translate(1, 0, 0);
        }
	}

    public void CheckMove()
    {
        if (!_Collision)
        {
            SpawnObj();
            transform.Rotate(0, 0, 90);
        }
        else
        {
            _Collision = false;
        }
    }

    public void SpawnObj()
    {
        if (_RuleSet[_Step].ToString() == "R")
            Instantiate(_PrefabObj_Right, _PrevPos, transform.rotation);
        else
            Instantiate(_PrefabObj_Left, _PrevPos, transform.rotation);

        _Step++;
        if (_Step + 1 > _MaxStep)
            _Step = 0;
    }

    void OnTriggerEnter(Collider other)
    {
        _Collision = true;
        Destroy(other.gameObject);


        if (_RuleSet[_Step].ToString() == "L" && other.gameObject.name.Contains("Left"))
            transform.Rotate(0, 0, -90);
        if (_RuleSet[_Step].ToString() == "R" && other.gameObject.name.Contains("Right"))
            transform.Rotate(0, 0, 90);
    }
}
