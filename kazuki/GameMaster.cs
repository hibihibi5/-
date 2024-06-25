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
    [Tooltip("���Ԑ����̃e�L�X�g")]
    [SerializeField] Text _timeRimitText = default;
    [Tooltip("�[����ʂł̎��Ԃ̑��x")]
    [SerializeField, Range(0, 1)] float _terminalTime = default;

    [Header("�R���|�[�l���g�擾")]
    [Tooltip("�J��������I�u�W�F�N�g")]
    [SerializeField] private CinemachineVirtualCamera _virtualCamera = default;

    private�@const float TIME_COUNT = 1; // ���{���̎���
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
                Time.timeScale = TIME_COUNT;
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

    // private--------------------------------------------------------------------------------------------------------------

    private void Start()
    {
        // ����������
        _timeRimitText.text = _timeRimit.ToString("F0"); // �L�����o�X�Ɏ��Ԑ����̏����l������
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
        if (_isGame && (_timeRimit > 0))
        {
            _timeRimit -= Time.deltaTime;
            _timeRimitText.text = _timeRimit.ToString("F0");
        }
        else
        {
            StopTime();
            print("�ύXyotei");
            _isGame = false; // �Q�[���̏I���t���O
            _timeRimitText.text = "0";
            _virtualCamera.enabled = false;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }
}
