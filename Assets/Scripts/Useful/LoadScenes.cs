using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScenes : MonoBehaviour
{
    public void Action_LoadScene(int sceneid)
    {
        SceneManager.LoadScene(sceneid);
    }
    public void Action_LoadScene(string scenename)
    {
        SceneManager.LoadScene(scenename);
    }

    public void Action_ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void Action_QuitApplication()
    {
        Application.Quit();
    }
}
