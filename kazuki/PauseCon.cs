using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseCon : MonoBehaviour
{
    [Header("�R���|�[�l���g�擾")]
    [Tooltip("�Q�[���}�X�^�[�I�u�W�F�N�g")]
    [SerializeField] GameMaster _gameMaster;
    [Tooltip("�|�[�Y��ʂ̃L�����o�X")]
    [SerializeField] private Canvas _pauseCanvas;

    private bool _isPause = false;

    private void Start()
    {
        // ����������
        _pauseCanvas.enabled = false; // �|�[�Y�L�����o�X�̔�\��
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
            bool isGame = _gameMaster.GetIsGame();

            if (isGame && !_isPause) // �A�N�e�B�u���
            {
                _isPause = true;
                _gameMaster.StopTime(); // ���Ԃ̒�~
                _pauseCanvas.enabled = true;
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }
            else if (isGame) // ��A�N�e�B�u���
            {
                _isPause = false;
                _gameMaster.NormalTime(); // ���Ԃ̍ĊJ
                _pauseCanvas.enabled = false;
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }
        }
    }
}
