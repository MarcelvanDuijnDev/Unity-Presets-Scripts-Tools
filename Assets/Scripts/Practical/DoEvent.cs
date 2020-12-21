using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DoEvent : MonoBehaviour
{
    [SerializeField] private UnityEvent _Event = null;
    [SerializeField] private bool _OnStart = false;
    [SerializeField] private bool _OnUpdate = false;
    [SerializeField] private bool _OnButtonPressed = false;

    void Start()
    {
        if (_OnStart)
            DoEvents();
    }

    void Update()
    {
        if (_OnUpdate)
            DoEvents();

        if (_OnButtonPressed)
            if (Input.anyKey)
                DoEvents();
    }

    private void DoEvents()
    {
        _Event.Invoke();
    }

    public void SetGameobject_InActive(GameObject targetobject)
    {
        targetobject.SetActive(false);
    }

    public void SetGameobject_Active(GameObject targetobject)
    {
        targetobject.SetActive(true);
    }

    public void SetGameObject_Negative(GameObject targetobject)
    {
        if (targetobject.activeSelf)
            targetobject.SetActive(false);
        else
            targetobject.SetActive(true);
    }

    public void LoadScene(int sceneid)
    {
        SceneManager.LoadScene(sceneid);
    }
    public void LoadScene(string scenename)
    {
        SceneManager.LoadScene(scenename);
    }
    public void Quit()
    {
        Application.Quit();
    }
}