using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseController1 : MonoBehaviour
{
    [SerializeField] private Canvas _pauseScreen;
    bool flag = true;
    private void Start()
    {
        _pauseScreen.gameObject.SetActive(false);
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            PauseGame();
        }


    }
    const int TIME_COUNT = 1;
    private void PauseGame()
    {
        if (flag)
        {
            Time.timeScale = 0;
            _pauseScreen.gameObject.SetActive(true);
            flag = false;
            Debug.Log(flag);

        }
        else
        {
            Time.timeScale = TIME_COUNT;
            _pauseScreen.gameObject.SetActive(false);
            flag = true;
            Debug.Log(flag);
        }

    }


}
