using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UnityPresetsNavigation : MonoBehaviour
{
    void Start()
    {
        
    }


    void Update()
    {
        
    }

    public void LoadScene(string scenename)
    {
        SceneManager.LoadScene(scenename);
    }
    public void LoadScene(int sceneid)
    {
        SceneManager.LoadScene(sceneid);
    }
}
