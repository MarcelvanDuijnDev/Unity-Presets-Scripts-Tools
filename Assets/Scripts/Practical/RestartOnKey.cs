using UnityEngine.SceneManagement;
using UnityEngine;

public class RestartOnKey : MonoBehaviour
{
    [SerializeField] private KeyCode _Key = KeyCode.R;

    void Update()
    {
        if(Input.GetKeyDown(_Key))
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
