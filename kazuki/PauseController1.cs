using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseController1 : MonoBehaviour
{
    [SerializeField] private Canvas _pauseScreen;

    const int TIME_COUNT = 1;

    private void Start()
    {
        // ����������
        _pauseScreen.enabled = false; // �|�[�Y�L�����o�X�̔�\��
    }

    private void Update()
    {
        // �|�[�Y��ʂ̕\��
        PauseGame();
    }

    /// <summary>
    /// �|�[�Y��ʂ̕\��
    /// </summary>
    private void PauseGame()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            // TimeScale��1�̂Ƃ�0��0�̂Ƃ�1��
            Time.timeScale = Time.timeScale == 1 ? Time.timeScale = 0 : Time.timeScale = TIME_COUNT;

            // �L�����o�X�̐؂�ւ�
            _pauseScreen.enabled = !_pauseScreen.enabled;
        }
    }
}
