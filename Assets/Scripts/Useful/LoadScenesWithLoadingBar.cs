using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using TMPro;

public class LoadScenesWithLoadingBar : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private TextMeshProUGUI _LoadingText;
    [SerializeField] private RectTransform _LoadingBarRect;

    private Vector2 _LoadingBarSize = Vector2.zero;
    private bool _AsyncLoading = false;

    private void Start()
    {
        _LoadingBarSize = _LoadingBarRect.sizeDelta;
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
            _LoadingText.text = (asyncLoad.progress * 100).ToString("0") + "%";
            _LoadingBarRect.sizeDelta = new Vector2(asyncLoad.progress * _LoadingBarSize.x, _LoadingBarSize.y);
            yield return null;
        }
    }
    private IEnumerator LoadSceneAsync(int sceneid)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneid);

        while (!asyncLoad.isDone)
        {
            _LoadingText.text = (asyncLoad.progress * 100).ToString("0") + "%";
            _LoadingBarRect.sizeDelta = new Vector2(asyncLoad.progress * _LoadingBarSize.x, _LoadingBarSize.y);
            yield return null;
        }
    }

    //Quit
    public void QuitApplication()
    {
        Application.Quit();
    }
}
