using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCon : MonoBehaviour
{
    [Tooltip("プレイヤーの移動速度")]
    [SerializeField] float _playerMoveSpeed = 0f;
    [Tooltip("重力の設定")]
    [SerializeField] Vector3 _playerGravity = new Vector3(0, -9.81f, 0);

    [Header("コンポーネント取得")]
    [SerializeField] Camera _padCamera;
    [SerializeField] GameObject _PadCanvas;
    GameObject _virtualCamera;
    Rigidbody _rb;
    private float _horizontalInput = 0;
    private float _verticalInput = 0;
    private bool _isMouseOn = false; // マウスの表示切替のbool
    private bool _isPadOpen = false; // パッド切替のbool

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _virtualCamera = GameObject.FindGameObjectWithTag("Virtual Camera");
    }

    private void Start()
    {
        // 初期化処理
        _rb.useGravity = false; // Rigidbodyの重力オフ
        Cursor.visible = false; // マウスの非表示
        Cursor.lockState = CursorLockMode.Locked; // マウスの中心固定
        _padCamera.enabled = false; // パッドカメラをオフに
        _PadCanvas.SetActive(false); // パッドキャンバスをオフに
    }

    private void Update()
    {
        // プレイヤーの移動処理
        PlayerMove();

        // マウスの表示処理
        MouseDisplay();

        // パッド画面の表示切替
        PadOpen();
    }

    private void FixedUpdate()
    {
        _rb.AddForce(_playerGravity, ForceMode.Acceleration); // 重力処理
    }

    /// <summary>
    /// プレイヤーの移動処理
    /// </summary>
    private void PlayerMove()
    {
        _horizontalInput = Input.GetAxisRaw("Horizontal");
        _verticalInput = Input.GetAxisRaw("Vertical");

        // 入力しているとき
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
        // ESCキーを押すとマウスの表示切替
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            _isMouseOn = !_isMouseOn;
        }

        // マウスの表示処理
        if (_isMouseOn) // 表示
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        else // 非表示
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    /// <summary>
    /// パッド画面の表示切替
    /// </summary>
    private void PadOpen()
    {
        // Eキーを押すとパッド画面切替
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (!_isPadOpen) // 表示
            {
                _padCamera.enabled = true;
                _virtualCamera.SetActive(false);
                _PadCanvas.SetActive(true);
                Camera.main.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
                _isMouseOn = true;
            }
            else // 非表示
            {
                _padCamera.enabled = false;
                _virtualCamera.SetActive(true);
                _PadCanvas.SetActive(false);
                _isMouseOn = false;
            }

            _isPadOpen = !_isPadOpen; // パッド切替のbool切替
        }
    }
}
