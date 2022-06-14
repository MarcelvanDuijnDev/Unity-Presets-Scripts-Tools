using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using UnityEngine;

public class DoEventOnInput : MonoBehaviour
{
    [Header("Input Settings")]
    [SerializeField] private KeyCode _InputKey;
    public enum InputOptions { GetKeyDown, GetKeyUp, GetKey }
    [SerializeField] private InputOptions _InputOption = InputOptions.GetKeyDown;

    [Header("Event")]
    [SerializeField] private UnityEvent _Event = null;

    [Header("Other Options")]
    [SerializeField] private bool _OnStart = false;
    [SerializeField] private bool _OnUpdate = false;
    [SerializeField] private bool _OnAnyKey = false;

    private bool _AsyncLoading = false;

    void Start()
    {
        if (_OnStart)
            DoEvents();
    }

    void Update()
    {
        if (_OnUpdate)
            DoEvents();

        if (_OnAnyKey)
            if (Input.anyKey)
                DoEvents();

        switch(_InputOption)
        {
            case InputOptions.GetKeyDown:
                if (Input.GetKeyDown(_InputKey))
                    _Event.Invoke();
                break;
            case InputOptions.GetKeyUp:
                if (Input.GetKeyUp(_InputKey))
                    _Event.Invoke();
                break;
            case InputOptions.GetKey:
                if (Input.GetKey(_InputKey))
                    _Event.Invoke();
                break;
        } 
    }

    private void DoEvents()
    {
        _Event.Invoke();
    }

    //Set Object true/false
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

    //Load/Reload Scenes
    public void LoadScene(int sceneid)
    {
        SceneManager.LoadScene(sceneid);
    }
    public void LoadScene(string scenename)
    {
        SceneManager.LoadScene(scenename);
    }
    public void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void AsyncReloadScene()
    {
        if (!_AsyncLoading)
        {
            _AsyncLoading = true;
            StartCoroutine(LoadSceneAsync(SceneManager.GetActiveScene().buildIndex));
        }
    }
    public void AsyncLoadScene(int sceneid)
    {
        if (!_AsyncLoading)
        {
            _AsyncLoading = true;
            StartCoroutine(LoadSceneAsync(sceneid));
        }
    }
    public void AsyncLoadScene(string scenename)
    {
        if (!_AsyncLoading)
        {
            _AsyncLoading = true;
            StartCoroutine(LoadSceneAsync(scenename));
        }
    }
    private IEnumerator LoadSceneAsync(string scenename)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(scenename);

        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }
    private IEnumerator LoadSceneAsync(int sceneid)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneid);

        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }

    //Quit
    public void Quit()
    {
        Application.Quit();
    }
}