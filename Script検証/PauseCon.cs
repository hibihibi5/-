using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.InputSystem;

public class PauseCon : MonoBehaviour
{
    [Header("�R���|�[�l���g�擾")]
    [Tooltip("�Q�[���}�X�^�[�I�u�W�F�N�g")]
    [SerializeField] GameMaster _gameMaster;
    [Tooltip("�[������X�N���v�g")]
    [SerializeField] TerminalCon _terminalCon;
    [Tooltip("�|�[�Y��ʂ̃L�����o�X")]
    [SerializeField] private Canvas _pauseCanvas;
    [Tooltip("�J��������I�u�W�F�N�g")]
    [SerializeField] private CinemachineVirtualCamera _virtualCamera = default;
    [SerializeField] private GameObject seObj;
    [Tooltip("�C���v�b�g�V�X�e���R���|�[�l���g�擾")]
    [SerializeField] PlayerInput _playerInput = default;

    InputAction _PauseInput = default;

    private bool _isPause = false;

    // public-------------------------------------------------------------------------------------------

    /// <summary>
    /// �|�[�Y��ʂ���鏈��
    /// </summary>
    public void PauseExit()
    {
        bool isGame = _gameMaster.GetIsGame();
        seObj.GetComponent<SEScript>().Cleck();

        if (isGame) // ��A�N�e�B�u���
        {
            _isPause = false;
            _gameMaster.GetPouseBool(_isPause);
            _gameMaster.PouseEscape();
            _pauseCanvas.enabled = false;

            bool isTerminal = _gameMaster.SetTerminalOpen();
            if (!isTerminal)
            {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
                _virtualCamera.enabled = true;
            }
        }
    }

    // private------------------------------------------------------------------------------------------

    private void Awake()
    {
        _PauseInput = _playerInput.actions["Pause"];
    }

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
        if (_PauseInput.WasPressedThisFrame())
        {
            bool isGame = _gameMaster.GetIsGame();

            if (isGame && !_isPause) // �A�N�e�B�u���
            {
                _isPause = true;
                _terminalCon.OutCancelNebanebaSet();
                _gameMaster.GetPouseBool(_isPause);
                _gameMaster.StopTime(); // ���Ԃ̒�~
                _pauseCanvas.enabled = true;
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                _virtualCamera.enabled = false;
            }
            else if (isGame)
            {
                PauseExit();
            }
        }
    }
}
