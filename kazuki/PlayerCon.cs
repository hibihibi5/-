using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCon : MonoBehaviour
{
    [Header("�ϐ��̎w��")]
    [Tooltip("�v���C���[�̈ړ����x")]
    [SerializeField] float _playerMoveSpeed = 0f;
    [Tooltip("�d�͂̐ݒ�")]
    [SerializeField] Vector3 _playerGravity = new Vector3(0, -9.81f, 0);
    [Tooltip("�[����ʂ�TimeScale")]
    [SerializeField, Range(0, 1)] float _timeScale = default;
    [Tooltip("�v���C���[�̉�]���x")]
    [SerializeField] float _rotationSpeed = 900f;
    [Tooltip("�U�����̃v���C���[�̈ړ����x")]
    [SerializeField, Range(0, 1)] float _setAttackMoveSpeed = 0.4f;

    [Header("�R���|�[�l���g�擾")]
    [SerializeField] GameMaster _gameMaster;
    [SerializeField] Camera _terminalCamera;
    [SerializeField] Canvas _terminalCanvas;
    [SerializeField] Canvas _mainCanvas;

    GameObject _virtualCamera;
    Rigidbody _rb;
    Animator _PlayerAnime;
    Quaternion _targetQuaternion; // ��]����p

    #region variable

    private Vector3 _moveForward = default;
    private float _horizontalInput = 0;
    private float _verticalInput = 0;
    private float _inputSpeed = default;
    private float _rotationSpeedCount = 0;
    private float _attackMoveSpeed = 1;
    private bool _isMouseOn = false; // �}�E�X�̕\���ؑւ�bool
    private bool _isTerminalOpen = false; // �[���ؑւ�bool

    #endregion

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _virtualCamera = GameObject.FindGameObjectWithTag("Virtual Camera");
        _PlayerAnime = this.gameObject.GetComponent<Animator>();
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
        //MouseDisplay();

        // �[����ʂ̕\���ؑ�
        TerminalOpen();

        // �U������
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
        if (!_isTerminalOpen)
        {
            _horizontalInput = Input.GetAxisRaw("Horizontal");
            _verticalInput = Input.GetAxisRaw("Vertical");
        }

        _rotationSpeedCount = _rotationSpeed * Time.deltaTime;

        // ���͂��Ă���Ƃ�
        if (_horizontalInput != 0 || _verticalInput != 0)
        {
            // �J�����̕�������AX-Z���ʂ̒P�ʃx�N�g�����擾
            Vector3 cameraForward = Vector3.Scale(Camera.main.transform.forward, new Vector3(1, 0, 1)).normalized;

            // �����L�[�̓��͒l�ƃJ�����̌�������A�ړ�����������
            _moveForward = cameraForward * _verticalInput + Camera.main.transform.right * _horizontalInput;
            _moveForward = _moveForward.normalized;

            // �ړ������ɃX�s�[�h���|����B
            _rb.velocity = _moveForward * _playerMoveSpeed * _attackMoveSpeed;

            // �L�����N�^�[�̌�����i�s������
            if (_moveForward != Vector3.zero)
            {
                _targetQuaternion = Quaternion.LookRotation(_moveForward);
            }

            if (_inputSpeed < 1)
            {
                _inputSpeed += Time.deltaTime * 2f;
            }
        }
        else
        {
            if (_inputSpeed > 0)
            {
                _inputSpeed -= Time.deltaTime * 2f;
            }
        }

        _PlayerAnime.SetFloat("Speed", _inputSpeed); // �ړ��A�j���[�V�����̍Đ�
        transform.rotation = Quaternion.RotateTowards(transform.rotation, _targetQuaternion, _rotationSpeedCount); // ��]���x�̐���
    }

    /// <summary>
    /// �[����ʂ̕\���ؑ�
    /// </summary>
    private void TerminalOpen()
    {
        // E�L�[�������ƒ[����ʐؑ�
        if (Input.GetKeyDown(KeyCode.E) && (Time.timeScale != 0))
        {
            if (!_isTerminalOpen) // �A�N�e�B�u���
            {
                _isTerminalOpen = true;
                _gameMaster.GetTerminalOpen(_isTerminalOpen); // �[����Ԃ̑��M
                _gameMaster.TerminalTime(); // ���Ԃ̑��x�ቺ

                // on
                _terminalCamera.enabled = true; // �J����
                _terminalCanvas.enabled = true; // �[���L�����o�X
                _isMouseOn = true;
                MouseDisplay();
                // off
                _mainCanvas.enabled = false; // ���C���L�����o�X
                _virtualCamera.SetActive(false); // �J�����̃R���g���[��
                _horizontalInput = 0;
                _verticalInput = 0;
            }
            else // ��A�N�e�B�u���
            {
                _isTerminalOpen = false;
                _gameMaster.GetTerminalOpen(_isTerminalOpen); // �[����Ԃ̑��M
                _gameMaster.NormalTime(); // ���Ԃ̊�{��Ԃ�

                // on
                _mainCanvas.enabled = true; // ���C���L�����o�X
                _virtualCamera.SetActive(true); // �J�����̃R���g���[��
                // off
                _terminalCamera.enabled = false; // �J����
                _terminalCanvas.enabled = false; // �[���L�����o�X
                _isMouseOn = false;
                MouseDisplay();
            }
        }
    }

    /// <summary>
    /// �}�E�X�̕\������
    /// </summary>
    private void MouseDisplay()
    {
        ////P�L�[�������ƃ}�E�X�̕\���ؑ�
        //if (Input.GetKeyDown(KeyCode.P))
        //{
        //    _isMouseOn = !_isMouseOn;
        //}

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
    /// �U������
    /// </summary>
    private void AttackCon()
    {
        // �[����ʂł͂Ȃ���Ԃō��N���b�N��������
        if (!_isTerminalOpen && Input.GetMouseButtonDown(0))
        {
            _isTerminalOpen = true;
            _PlayerAnime.SetBool("attack", true);

            _attackMoveSpeed = _setAttackMoveSpeed;
        }
    }

    /// <summary>
    /// �U���̏I������
    /// </summary>
    public void AttackEnd()
    {
        _PlayerAnime.SetBool("attack", false);
        _isTerminalOpen = false;

        _attackMoveSpeed = 1;
    }
}
