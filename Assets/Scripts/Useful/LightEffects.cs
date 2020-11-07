using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightEffects : MonoBehaviour
{
    public enum LightEffectOptions { Flickering, Off, On };

    [Header("Settings")]
    [SerializeField] private LightEffectOptions _LightEffectOption = LightEffectOptions.Flickering;
    [SerializeField] private Vector2 _MinMaxIncrease = new Vector2(0.8f, 1.2f);
    [Range(0.01f, 100)] [SerializeField] private float _EffectStrength = 50;

    Queue<float> _LightFlickerQ;
    private float _LastSum = 0;
    private Light _Light;
    private float _LightIntensity = 0;

    public void Reset()
    {
        if (_LightEffectOption == LightEffectOptions.Flickering)
        {
            _LightFlickerQ.Clear();
            _LastSum = 0;
        }
    }

    void Start()
    {
        _Light = GetComponent<Light>();
        _LightIntensity = _Light.intensity;
        _LightFlickerQ = new Queue<float>(Mathf.RoundToInt(_EffectStrength));
    }

    void Update()
    {
        switch(_LightEffectOption)
        {
            case LightEffectOptions.Flickering:
                while (_LightFlickerQ.Count >= _EffectStrength)
                    _LastSum -= _LightFlickerQ.Dequeue();

                float newVal = Random.Range(_LightIntensity * _MinMaxIncrease.x, _LightIntensity * _MinMaxIncrease.y);
                _LightFlickerQ.Enqueue(newVal);
                _LastSum += newVal;
                _Light.intensity = _LastSum / (float)_LightFlickerQ.Count;
                break;
            case LightEffectOptions.Off:
                _Light.intensity = 0;
                break;
            case LightEffectOptions.On:
                _Light.intensity = _LightIntensity = _MinMaxIncrease.x;
                break;
        }

    }

    public void SetEffect(LightEffectOptions options)
    {
        _LightEffectOption = options;
    }
}