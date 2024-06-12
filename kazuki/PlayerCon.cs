using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCon : MonoBehaviour
{
    [Tooltip("プレイヤーの移動速度")]
    [SerializeField] float _playerMoveSpeed = 0f;
    [Tooltip("重力の設定")]
    [SerializeField] Vector3 _playerGravity = new Vector3(0, -9.81f, 0);
    [Tooltip("端末画面のTimeScale")]
    [SerializeField, Range(0, 1)] float _timeScale = default;

    [Header("コンポーネント取得")]
    [SerializeField] Camera _terminalCamera;
    [SerializeField] Canvas _terminalCanvas;
    [SerializeField] Animator _attackAnime;

    GameObject _virtualCamera;
    Rigidbody _rb;
    Quaternion _targetQuaternion; // 回転制御用

    #region variable

    private float _horizontalInput = 0;
    private float _verticalInput = 0;
    private bool _isMouseOn = false; // マウスの表示切替のbool
    private bool _isTerminalOpen = false; // パッド切替のbool

    #endregion

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
        _terminalCamera.enabled = false; // 端末カメラをオフに
        _terminalCanvas.enabled = false; // 端末キャンバスをオフに
        _terminalCanvas.gameObject.GetComponent<TerminalCon>().enabled = true; // 端末操作スクリプトをオフに

        _targetQuaternion = transform.rotation;
    }

    private void Update()
    {
        // プレイヤーの移動処理
        PlayerMove();

        // マウスの表示処理
        MouseDisplay();

        // 端末画面の表示切替
        PadOpen();

        AttackCon();
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

        var rotationSpeed = 600 * Time.deltaTime;

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
                _targetQuaternion = Quaternion.LookRotation(moveForward);
            }
            transform.rotation = Quaternion.RotateTowards(transform.rotation, _targetQuaternion, rotationSpeed); // 回転速度の制限
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
    /// 端末画面の表示切替
    /// </summary>
    private void PadOpen()
    {
        // Eキーを押すと端末画面切替
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (!_isTerminalOpen) // 表示
            {
                Time.timeScale = _timeScale;
                _terminalCamera.enabled = true;
                _terminalCanvas.enabled = true;
                _terminalCanvas.gameObject.GetComponent<TerminalCon>().enabled = true;
                _virtualCamera.SetActive(false);
                Camera.main.transform.rotation = Quaternion.Euler(Vector3.zero);
                _isMouseOn = true;
            }
            else // 非表示
            {
                Time.timeScale = 1;
                _virtualCamera.SetActive(true);
                _terminalCamera.enabled = false;
                _terminalCanvas.enabled = false;
                _terminalCanvas.gameObject.GetComponent<TerminalCon>().enabled = false;
                _isMouseOn = false;
            }

            _isTerminalOpen = !_isTerminalOpen; // 端末画面切替のbool切替
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
