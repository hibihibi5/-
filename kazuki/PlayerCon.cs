using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCon : MonoBehaviour
{
    [SerializeField] float _playerMoveSpeed = 0f;
    [SerializeField] Vector3 _playerGravity = new Vector3(0, -9.81f, 0);
    Rigidbody _rb;
    private float _horizontalInput = 0;
    private float _verticalInput = 0;
    private bool _isMouseOn = false;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.useGravity = false;
    }

    private void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        PlayerMove();

        MouseDisplay();
    }

    private void FixedUpdate()
    {
        _rb.AddForce(_playerGravity, ForceMode.Acceleration);
    }

    /// <summary>
    /// プレイヤーの移動処理
    /// </summary>
    private void PlayerMove()
    {
        _horizontalInput = Input.GetAxisRaw("Horizontal");
        _verticalInput = Input.GetAxisRaw("Vertical");

        if (_horizontalInput != 0 || _verticalInput != 0)
        {
            // カメラの方向から、X-Z平面の単位ベクトルを取得
            Vector3 cameraForward = Vector3.Scale(Camera.main.transform.forward, new Vector3(1, 0, 1)).normalized;

            // 方向キーの入力値とカメラの向きから、移動方向を決定
            Vector3 moveForward = cameraForward * _verticalInput + Camera.main.transform.right * _horizontalInput;
            moveForward = moveForward.normalized;

            // 移動方向にスピードを掛ける。ジャンプや落下がある場合は、別途Y軸方向の速度ベクトルを足す。
            _rb.velocity = moveForward * _playerMoveSpeed;

            // キャラクターの向きを進行方向に
            if (moveForward != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(moveForward);
            }
        }
    }

    /// <summary>
    /// マウスの表示処理
    /// </summary>
    private void MouseDisplay()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            _isMouseOn = !_isMouseOn;
        }

        // マウスの表示処理
        if (_isMouseOn)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
}
