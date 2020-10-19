using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DoEvent : MonoBehaviour
{
    [SerializeField] private UnityEvent _Event;
    [SerializeField] private bool _OnStart;
    [SerializeField] private bool _OnUpdate;
    [SerializeField] private bool _OnButtonPressed;

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