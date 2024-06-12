using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCon : MonoBehaviour
{
    [Tooltip("�v���C���[�̈ړ����x")]
    [SerializeField] float _playerMoveSpeed = 0f;
    [Tooltip("�d�͂̐ݒ�")]
    [SerializeField] Vector3 _playerGravity = new Vector3(0, -9.81f, 0);
    [Tooltip("�[����ʂ�TimeScale")]
    [SerializeField, Range(0, 1)] float _timeScale = default;

    [Header("�R���|�[�l���g�擾")]
    [SerializeField] Camera _terminalCamera;
    [SerializeField] Canvas _terminalCanvas;
    [SerializeField] Animator _attackAnime;

    GameObject _virtualCamera;
    Rigidbody _rb;
    Quaternion _targetQuaternion; // ��]����p

    #region variable

    private float _horizontalInput = 0;
    private float _verticalInput = 0;
    private bool _isMouseOn = false; // �}�E�X�̕\���ؑւ�bool
    private bool _isTerminalOpen = false; // �p�b�h�ؑւ�bool

    #endregion

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _virtualCamera = GameObject.FindGameObjectWithTag("Virtual Camera");
    }

    private void Start()
    {
        // ����������
        _rb.useGravity = false; // Rigidbody�̏d�̓I�t
        Cursor.visible = false; // �}�E�X�̔�\��
        Cursor.lockState = CursorLockMode.Locked; // �}�E�X�̒��S�Œ�
        _terminalCamera.enabled = false; // �[���J�������I�t��
        _terminalCanvas.enabled = false; // �[���L�����o�X���I�t��
        _terminalCanvas.gameObject.GetComponent<TerminalCon>().enabled = true; // �[������X�N���v�g���I�t��

        _targetQuaternion = transform.rotation;
    }

    private void Update()
    {
        // �v���C���[�̈ړ�����
        PlayerMove();

        // �}�E�X�̕\������
        MouseDisplay();

        // �[����ʂ̕\���ؑ�
        PadOpen();

        AttackCon();
    }

    private void FixedUpdate()
    {
        _rb.AddForce(_playerGravity, ForceMode.Acceleration); // �d�͏���
    }

    /// <summary>
    /// �v���C���[�̈ړ�����
    /// </summary>
    private void PlayerMove()
    {
        _horizontalInput = Input.GetAxisRaw("Horizontal");
        _verticalInput = Input.GetAxisRaw("Vertical");

        var rotationSpeed = 600 * Time.deltaTime;

        // ���͂��Ă���Ƃ�
        if (_horizontalInput != 0 || _verticalInput != 0)
        {
            // �J�����̕�������AX-Z���ʂ̒P�ʃx�N�g�����擾
            Vector3 cameraForward = Vector3.Scale(Camera.main.transform.forward, new Vector3(1, 0, 1)).normalized;

            // �����L�[�̓��͒l�ƃJ�����̌�������A�ړ�����������
            Vector3 moveForward = cameraForward * _verticalInput + Camera.main.transform.right * _horizontalInput;
            moveForward = moveForward.normalized;

            // �ړ������ɃX�s�[�h���|����B�W�����v�◎��������ꍇ�́A�ʓrY�������̑��x�x�N�g���𑫂��B
            _rb.velocity = moveForward * _playerMoveSpeed;

            // �L�����N�^�[�̌�����i�s������
            if (moveForward != Vector3.zero)
            {
                _targetQuaternion = Quaternion.LookRotation(moveForward);
            }
            transform.rotation = Quaternion.RotateTowards(transform.rotation, _targetQuaternion, rotationSpeed); // ��]���x�̐���
        }
    }

    /// <summary>
    /// �}�E�X�̕\������
    /// </summary>
    private void MouseDisplay()
    {
        // ESC�L�[�������ƃ}�E�X�̕\���ؑ�
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            _isMouseOn = !_isMouseOn;
        }

        // �}�E�X�̕\������
        if (_isMouseOn) // �\��
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        else // ��\��
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    /// <summary>
    /// �[����ʂ̕\���ؑ�
    /// </summary>
    private void PadOpen()
    {
        // E�L�[�������ƒ[����ʐؑ�
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (!_isTerminalOpen) // �\��
            {
                Time.timeScale = _timeScale;
                _terminalCamera.enabled = true;
                _terminalCanvas.enabled = true;
                _terminalCanvas.gameObject.GetComponent<TerminalCon>().enabled = true;
                _virtualCamera.SetActive(false);
                Camera.main.transform.rotation = Quaternion.Euler(Vector3.zero);
                _isMouseOn = true;
            }
            else // ��\��
            {
                Time.timeScale = 1;
                _virtualCamera.SetActive(true);
                _terminalCamera.enabled = false;
                _terminalCanvas.enabled = false;
                _terminalCanvas.gameObject.GetComponent<TerminalCon>().enabled = false;
                _isMouseOn = false;
            }

            _isTerminalOpen = !_isTerminalOpen; // �[����ʐؑւ�bool�ؑ�
        }
    }

    private void AttackCon()
    {
        if (!_isTerminalOpen && Input.GetMouseButtonDown(0))
        {
            _attackAnime.SetTrigger("Attack");
        }
    }
}
