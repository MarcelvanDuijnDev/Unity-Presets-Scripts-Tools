using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;

public class RandomEvent : MonoBehaviour
{
    [SerializeField] private Vector2 _RandomMinMax;
    [SerializeField] private UnityEvent _Event;

    private float _Timer;
    private float _NextGoal;

    void Start()
    {
        _NextGoal = Random.Range(_RandomMinMax.x, _RandomMinMax.y);
    }

    void Update()
    {
        _Timer += 1 * Time.deltaTime;
        if (_Timer >= _NextGoal)
        {
            _Event.Invoke();
            _Timer = 0;
            _NextGoal = _NextGoal = Random.Range(_RandomMinMax.x, _RandomMinMax.y);
        }
    }

    public void AudioHandlerEvent(string audiotrack)
    {
        AudioHandler.AUDIO.PlayTrack(audiotrack);
    }
}
