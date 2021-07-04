using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject _PauseMenu;

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            _PauseMenu.SetActive(!_PauseMenu.activeSelf);

            if (_PauseMenu.activeSelf)
                Time.timeScale = 0;
            else
                Time.timeScale = 1;
        }
    }

    public void LoadScene(int sceneid)
    {
        SceneManager.LoadScene(sceneid);
        Time.timeScale = 1;
    }

    public void LoadScene(string scenename)
    {
        SceneManager.LoadScene(scenename);
        Time.timeScale = 1;
    }

    public void Resume()
    {
        _PauseMenu.SetActive(false);
        Time.timeScale = 1;
    }
}
