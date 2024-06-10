using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCon : MonoBehaviour
{
    [Tooltip("�v���C���[�̈ړ����x")]
    [SerializeField] float _playerMoveSpeed = 0f;
    [Tooltip("�d�͂̐ݒ�")]
    [SerializeField] Vector3 _playerGravity = new Vector3(0, -9.81f, 0);

    [Header("�R���|�[�l���g�擾")]
    [SerializeField] Camera _padCamera;
    [SerializeField] GameObject _PadCanvas;
    GameObject _virtualCamera;
    Rigidbody _rb;
    private float _horizontalInput = 0;
    private float _verticalInput = 0;
    private bool _isMouseOn = false; // �}�E�X�̕\���ؑւ�bool
    private bool _isPadOpen = false; // �p�b�h�ؑւ�bool

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
        _padCamera.enabled = false; // �p�b�h�J�������I�t��
        _PadCanvas.SetActive(false); // �p�b�h�L�����o�X���I�t��
    }

    private void Update()
    {
        // �v���C���[�̈ړ�����
        PlayerMove();

        // �}�E�X�̕\������
        MouseDisplay();

        // �p�b�h��ʂ̕\���ؑ�
        PadOpen();
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
                transform.rotation = Quaternion.LookRotation(moveForward);
            }
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
    /// �p�b�h��ʂ̕\���ؑ�
    /// </summary>
    private void PadOpen()
    {
        // E�L�[�������ƃp�b�h��ʐؑ�
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (!_isPadOpen) // �\��
            {
                _padCamera.enabled = true;
                _virtualCamera.SetActive(false);
                _PadCanvas.SetActive(true);
                Camera.main.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
                _isMouseOn = true;
            }
            else // ��\��
            {
                _padCamera.enabled = false;
                _virtualCamera.SetActive(true);
                _PadCanvas.SetActive(false);
                _isMouseOn = false;
            }

            _isPadOpen = !_isPadOpen; // �p�b�h�ؑւ�bool�ؑ�
        }
    }
}
