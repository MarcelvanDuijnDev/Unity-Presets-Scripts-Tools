using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine;

public class ExampleGame_TopDownShooter : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI _ScoreText;
    [SerializeField] private TextMeshProUGUI _AliveTimeText;
    [Header("Set")]
    [SerializeField] private GameObject _HealthParent;
    [SerializeField] private GameObject _GameOverMenu;

    [Header("Health")]
    [SerializeField] private int _MaxHealth;
    [SerializeField] private int _CurrentHealth;

    private float _Score;
    private float _AliveTime;

    private List<GameObject> _HealthObj = new List<GameObject>();

    private void Start()
    {
        _GameOverMenu.SetActive(false);
        for (int i = 0; i < _HealthParent.transform.childCount; i++)
        {
            _HealthObj.Add(_HealthParent.transform.GetChild(i).gameObject);
            if (i < _CurrentHealth) { }
            else
                _HealthObj[i].SetActive(false);
        }
    }

    void Update()
    {
        _AliveTime += 1 * Time.deltaTime;

        _ScoreText.text = _Score.ToString("0");
        _AliveTimeText.text = string.Format("{0:00}:{1:00}:{2:00}", Mathf.Floor(_AliveTime / 3600), Mathf.Floor((_AliveTime / 60) % 60), _AliveTime % 60);




        //Restart
        if (Input.GetKeyDown(KeyCode.R))
        {
            Time.timeScale = 1;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    public void AddScore(float amount)
    {
        _Score += amount;
    }
    public void AddHealth()
    {
        _HealthObj[_CurrentHealth].SetActive(true);
        _CurrentHealth++;
    }
    public void DoDamage()
    {
        if (_CurrentHealth > 0)
        {
            _CurrentHealth--;
            _HealthObj[_CurrentHealth].SetActive(false);
        }

        if (_CurrentHealth <= 0)
        {
            Time.timeScale = 0;
            _GameOverMenu.SetActive(true);
        }
    }
}
