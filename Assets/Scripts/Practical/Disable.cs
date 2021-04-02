using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Disable : MonoBehaviour {

    [SerializeField] private GameObject _Object;

    public void DisableObject(float seconds) {
        StartCoroutine(StartDisable(seconds));
    }

    private IEnumerator StartDisable(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        _Object.SetActive(false);
    }
}
