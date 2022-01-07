using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SolarSystem : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float _SpeedMultiplier = 1;

    [Header("Planets")]
    [SerializeField] private List<SolarSystem_Planet> _Planet = new List<SolarSystem_Planet>();

    private void Start()
    {
        for (int i = 0; i < _Planet.Count; i++)
        {
            _Planet[i].CalculatedSpeed = _Planet[i].Days * 0.4115f;
        }
    }

    void Update()
    {
        for (int i = 0; i < _Planet.Count; i++)
        {
            _Planet[i].Center.Rotate(new Vector3(_Planet[i].RotationSpeed.x, _Planet[i].RotationSpeed.y, _Planet[i].RotationSpeed.z) * _Planet[i].CalculatedSpeed * _SpeedMultiplier * Time.deltaTime);
        }
    }
}

[System.Serializable]
public class SolarSystem_Planet
{
    public string Names;
    public Transform Center;
    public Vector3 RotationSpeed;
    public float Days;
    [HideInInspector] public float CalculatedSpeed;
}