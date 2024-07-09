using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.InputSystem;

public class PlayerCon : MonoBehaviour
{
    #region setVariable

    [Header("�ϐ��̎w��")]
    [Tooltip("�v���C���[�̒ʏ펞�̈ړ����x")]
    [SerializeField] float _playerNormalMoveSpeed = 15f;
    //[Tooltip("�v���C���[�̃_�b�V�����̈ړ����x")]
    //[SerializeField] float _playerDashSpeed = default;
    [Tooltip("�v���C���[�̈ړ��̕ω����x")]
    [SerializeField] float _playerMoveChegeSpeed = default;
    [Tooltip("�d�͂̐ݒ�")]
    [SerializeField] Vector3 _playerGravity = new Vector3(0, -200f, 0);
    [Tooltip("�v���C���[�̉�]���x")]
    [SerializeField] float _rotationSpeed = 900f;
    [Tooltip("�U�����̃v���C���[�̈ړ����x")]
    [SerializeField, Range(0, 1)] float _setAttackMoveSpeed = 0.5f;
    [Tooltip("�˂΂˂Ώ��ł̑��x")]
    [SerializeField, Range(0, 1)] float _nebanebaSpeed = 0.4f;
    [Tooltip("�ړ��A�j���[�V�����̕ω��̑��x")]
    [SerializeField] float _moveAnimeChegeSpeed = 1;
    [Tooltip("���e�̐ݒu�Ɋ|���鎞��")]
    [SerializeField] float _BombSetTime = default;
    [Tooltip("���e���ǂ��ɐ������邩")]
    [SerializeField] float _bombSetPoint = default;

    #endregion

    #region setComponent

    [Header("�R���|�[�l���g�擾")]
    [SerializeField] GameMaster _gameMaster;
    [SerializeField] Camera _terminalCamera;
    [SerializeField] Canvas _terminalCanvas;
    [SerializeField] Canvas _mainCanvas;
    [Tooltip("�J��������I�u�W�F�N�g")]
    [SerializeField] private CinemachineVirtualCamera _virtualCamera = default;
    [Tooltip("�C���v�b�g�V�X�e���R���|�[�l���g�擾")]
    [SerializeField] PlayerInput _playerInput = default;
    [SerializeField] private GameObject seObj;

    #endregion

    #region component

    Rigidbody _rb;
    Animator _playerAnime;
    Quaternion _targetQuaternion; // ��]����p
    TerminalCon _terminalCon = default;
    InputAction _moveInput = default;
    InputAction _attackInput = default;
    InputAction _terminalInput = default;


    #endregion

    #region variable

    private const int ONE = 1;
    private Vector3 _moveForward = default;
    private float _horizontalInput = 0;
    private float _verticalInput = 0;
    private float _inputSpeed = default;
    private float _rotationSpeedCount = 0;
    private float _attackMoveSpeed = 1;
    private float _externalSpeed = 1;
    private float _BombSetTimeNow = default;
    private float _playerMoveSpeedNow = default; // ���݂̃v���C���[�̈ړ����x
    private float _inputDeshSpeed = 1;
    private bool _isMouseOn = false; // �}�E�X�̕\���ؑւ�bool
    private bool _isTerminalOpen = false; // �[���ؑւ�bool
    private bool _isAttack = false;
    private bool _isBombSet = false;
    private bool _isPlayerDesh = false;

    #endregion

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _playerAnime = this.gameObject.GetComponent<Animator>();
        _terminalCon = _terminalCanvas.gameObject.GetComponent<TerminalCon>();

        _moveInput = _playerInput.actions["Move"];
        _attackInput = _playerInput.actions["Attack"];
        _terminalInput = _playerInput.actions["Terminal"];
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

        // ���e�̐ݒu�J�n����
        BombSetStart();
    }

    private void FixedUpdate()
    {
        _rb.AddForce(_playerGravity, ForceMode.Acceleration); // �d�͏���

        BombSet();
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Nebaneba"))
        {
            _externalSpeed = _nebanebaSpeed;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        _externalSpeed = ONE;
    }

    /// <summary>
    /// �v���C���[�̈ړ�����
    /// </summary>
    private void PlayerMove()
    {
        if (!_isTerminalOpen && !_isBombSet && !_gameMaster.SetPouseBool())
        {
            var inputMoveAxis = _moveInput.ReadValue<Vector2>();
            _horizontalInput = inputMoveAxis.x;
            _verticalInput = inputMoveAxis.y;

            //if (!Input.GetKey(KeyCode.LeftShift)) // �m�[�}��
            //{
            //    _isPlayerDesh = false;
            //}
            //else // �_�b�V��
            //{
            //    _isPlayerDesh = true;
            //}
        }

        _rotationSpeedCount = _rotationSpeed * Time.deltaTime;

        // ���͂��Ă���Ƃ�
        if (_horizontalInput != 0 || _verticalInput != 0)
        {
            //if (_isPlayerDesh && (_playerMoveSpeedNow < _playerDashSpeed))
            //{
            //    _playerMoveSpeedNow += Time.deltaTime * _playerMoveChegeSpeed;
            //}
            //else if (!_isPlayerDesh && (_playerMoveSpeedNow > _playerDashSpeed))
            //{
            //    _playerMoveSpeedNow -= Time.deltaTime * _playerMoveChegeSpeed;
            //}
            //else if (!_isPlayerDesh)
            //{
            //    _playerMoveSpeedNow = _playerNormalMoveSpeed;
            //}

            // �J�����̕�������AX-Z���ʂ̒P�ʃx�N�g�����擾
            Vector3 cameraForward = Vector3.Scale(Camera.main.transform.forward, new Vector3(1, 0, 1)).normalized;

            // �����L�[�̓��͒l�ƃJ�����̌�������A�ړ�����������
            _moveForward = cameraForward * _verticalInput + Camera.main.transform.right * _horizontalInput;
            _moveForward = _moveForward.normalized;

            // �ړ������ɃX�s�[�h���|����B
            _rb.velocity = _moveForward * _playerNormalMoveSpeed * _attackMoveSpeed * _externalSpeed;

            // �L�����N�^�[�̌�����i�s������
            if (_moveForward != Vector3.zero)
            {
                _targetQuaternion = Quaternion.LookRotation(_moveForward);
            }

            //// �A�j���[�V�����̑J�ڏ���
            //if (_isPlayerDesh && (_inputDeshSpeed < 2.5f))
            //{
            //    _inputDeshSpeed += Time.deltaTime * _moveAnimeChegeSpeed;
            //}
            //else if (!_isPlayerDesh && (_inputDeshSpeed > 1))
            //{
            //    _inputDeshSpeed -= Time.deltaTime * _moveAnimeChegeSpeed;
            //}
            //else if (!_isPlayerDesh)
            //{
            //    _inputDeshSpeed = 1;
            //}

            //if (_inputSpeed < 0.4f) // �m�[�}��
            //{
            //    _inputSpeed += Time.deltaTime * _moveAnimeChegeSpeed;
            //}

            if (_inputSpeed < ONE)
            {
                _inputSpeed += Time.deltaTime * _moveAnimeChegeSpeed;
            }
            else
            {
                _inputSpeed = ONE;
            }
        }
        else
        {
            //if (_isPlayerDesh && (_playerMoveSpeedNow > _playerNormalMoveSpeed))
            //{
            //    _playerMoveSpeedNow -= Time.deltaTime * _playerMoveChegeSpeed;
            //}
            //else if (_isPlayerDesh)
            //{
            //    _playerMoveSpeedNow = _playerNormalMoveSpeed;
            //}

            //if (_inputSpeed > 0)
            //{
            //    _inputSpeed -= Time.deltaTime * _moveAnimeChegeSpeed;
            //}

            if (_inputSpeed > 0)
            {
                _inputSpeed -= Time.deltaTime * _moveAnimeChegeSpeed;
            }
            else
            {
                _inputSpeed = 0;
            }
        }

        _playerAnime.SetFloat("Speed", _inputSpeed/* * _inputDeshSpeed*/); // �ړ��A�j���[�V�����̍Đ�
        _rb.MoveRotation(Quaternion.RotateTowards(transform.rotation, _targetQuaternion, _rotationSpeedCount)); // ��]���x�̐���
    }

    /// <summary>
    /// �[����ʂ̕\���ؑ�
    /// </summary>
    private void TerminalOpen()
    {
        // E�L�[�������ƒ[����ʐؑ�
        if (_terminalInput.WasPressedThisFrame() && !_gameMaster.SetPouseBool() && _gameMaster.GetIsGame())
        {
            if (!_isTerminalOpen && !_isBombSet) // �A�N�e�B�u���
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
                _virtualCamera.enabled = false; // �J�����̃R���g���[��
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
                _virtualCamera.enabled=true; // �J�����̃R���g���[��
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
        if (!_isTerminalOpen && !_isAttack && _attackInput.WasPressedThisFrame())
        {
            if (!_gameMaster.SetPouseBool() && _playerAnime.GetCurrentAnimatorStateInfo(0).IsName("Move"))
            {
                _isAttack = true;
                _playerAnime.SetBool("attack", true);
                seObj.GetComponent<SEScript>().PLATK();
                _attackMoveSpeed = _setAttackMoveSpeed;
            }
        }
    }

    /// <summary>
    /// �U���̏I������
    /// </summary>
    public void AttackEnd()
    {
        _playerAnime.SetBool("attack", false);
        _isAttack = false;

        _attackMoveSpeed = ONE;
    }

    /// <summary>
    /// ���e�̐ݒu�J�n����
    /// </summary>
    private void BombSetStart()
    {
        // �[����ʂł͂Ȃ���Ԃō��N���b�N��������
        if (!_isTerminalOpen && !_isBombSet && Input.GetMouseButtonDown(1))
        {
            if (_terminalCon.GetBombCost() && !_gameMaster.SetPouseBool())
            {
                _playerAnime.SetBool("BombSet", true);

                _horizontalInput = 0;
                _verticalInput = 0;
                _isBombSet = true;
                _BombSetTimeNow = _BombSetTime;
            }
            else
            {
                print("kosutogatarinai");
            }
        }
    }

    /// <summary>
    /// ���e�̐ݒu����
    /// </summary>
    private void BombSet()
    {
        if (_isBombSet)
        {
            _BombSetTimeNow -= Time.deltaTime;

            if (_BombSetTimeNow <= 0)
            {
                Instantiate(Resources.Load("Bomb"), this.transform.position + this.transform.forward * _bombSetPoint, this.transform.localRotation);

                _isBombSet = false;
                _playerAnime.SetBool("BombSet", false);
            }
        }
    }
}
