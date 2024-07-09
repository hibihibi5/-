using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;

public class GameMaster : MonoBehaviour
{
    [Header("�ϐ��̎w��")]
    [Tooltip("���Ԑ���")]
    [SerializeField] float _timeRimit = default;
    [Tooltip("�[����ʂł̎��Ԃ̑��x")]
    [SerializeField, Range(0, 1)] float _terminalTime = default;
    [Tooltip("�Q�[���I�����̎��Ԃ̑��x")]
    [SerializeField, Range(0, 1)] float _endGameTime = default;

    [Header("�R���|�[�l���g�擾")]
    [Tooltip("���C����ʂ̎��Ԑ����̃e�L�X�g")]
    [SerializeField] Text _mainTimeRimitText = default;
    [Tooltip("�[����ʂ̎��Ԑ����e�L�X�g")]
    [SerializeField] Text _tarminalTimeRimitText = default;
    //[Tooltip("�Q�[�������̃L�����o�X")]
    //[SerializeField] Canvas _explanationCanvas = default;
    [Tooltip("�J��������I�u�W�F�N�g")]
    [SerializeField] private CinemachineVirtualCamera _virtualCamera = default;

    private�@const float NORMAL_TIME = 1; // ���{���̎���
    private bool _isGame = true; // �Q�[�����i�s���Ă��邩�ǂ���
    private bool _isTerminalOpen = false; // �[�����N�����Ă��邩�ǂ���
    private bool _isPause = false; // �|�[�Y��Ԃ��ǂ���

    // public------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// ���Ԃ���{���x�ɂ���
    /// </summary>
    public void NormalTime()
    {
        if (_isGame)
        {
            if (!_isTerminalOpen) // �[����Ԃł͖���
            {
                Time.timeScale = NORMAL_TIME;
            }
            else�@// �[�����
            {
                TerminalTime();
            }
        }
    }

    /// <summary>
    /// ���Ԃ��~����
    /// </summary>
    public void StopTime()
    {
        if (_isGame)
        {
            Time.timeScale = 0;
        }
    }

    /// <summary>
    /// ���Ԃ�[����ʂł̑��x�ɂ���
    /// </summary>
    public void TerminalTime()
    {
        if (_isGame)
        {
            Time.timeScale = _terminalTime;
        }
    }

    /// <summary>
    /// �|�[�Y��Ԃ�����
    /// </summary>
    public void PouseEscape()
    {
        if (!_isTerminalOpen) // �ʏ�
        {
            NormalTime();
        }
        else // �[��
        {
            TerminalTime();
        }
    }

    /// <summary>
    /// �[���̏�Ԏ擾
    /// </summary>
    /// <param name="terminalBool"></param>
    public void GetTerminalOpen(bool terminalBool)
    {
        _isTerminalOpen = terminalBool;
    }

    /// <summary>
    /// �[���̏�Ԃ�Ԃ�
    /// </summary>
    /// <returns></returns>
    public bool SetTerminalOpen()
    {
        return _isTerminalOpen;
    }

    /// <summary>
    /// �|�[�Y�̏�Ԏ擾
    /// </summary>
    /// <param name="isPouse"></param>
    public void GetPouseBool(bool isPouse)
    {
        _isPause = isPouse;
    }

    /// <summary>
    /// �|�[�Y�̏�Ԃ�Ԃ�
    /// </summary>
    /// <returns></returns>
    public bool SetPouseBool()
    {
        return _isPause;
    }

    /// <summary>
    /// �Q�[���̎��s��Ԃ�Ԃ�
    /// </summary>
    /// <returns></returns>
    public bool GetIsGame()
    {
        return _isGame;
    }

    /// <summary>
    /// �Q�[���̎��s��Ԃ��擾
    /// </summary>
    public void SetGameOver()
    {
        _isGame = false;
    }

    // private--------------------------------------------------------------------------------------------------------------

    private void Start()
    {
        // ����������
        Time.timeScale = NORMAL_TIME;
        _mainTimeRimitText.text = _timeRimit.ToString("F0"); // �L�����o�X�Ɏ��Ԑ����̏����l������
        _tarminalTimeRimitText.text = _timeRimit.ToString("F0"); // �L�����o�X�Ɏ��Ԑ����̏����l������

    }

    private void FixedUpdate()
    {
        TimeCount();
    }

    /// <summary>
    /// ���Ԑ����̏���
    /// </summary>
    private void TimeCount()
    {
        if (_isGame && !_isPause && (_timeRimit > 0))
        {
            _timeRimit -= Time.deltaTime;
            _mainTimeRimitText.text = _timeRimit.ToString("F0");
            _tarminalTimeRimitText.text = _timeRimit.ToString("F0");
        }
        else if (!_isPause)
        {
            Time.timeScale = _endGameTime;
            _isGame = false; // �Q�[���̏I���t���O
            _mainTimeRimitText.text = "0";
            _tarminalTimeRimitText.text = "0";
            _virtualCamera.enabled = false;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }
}
