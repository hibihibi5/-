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
    [Tooltip("プレイヤーの回転速度")]
    [SerializeField] float _rotationSpeed = 600;

    [Header("コンポーネント取得")]
    [SerializeField] Camera _terminalCamera;
    [SerializeField] Canvas _terminalCanvas;
    [SerializeField] Canvas _mainCanvas;
    [SerializeField] Animator _attackAnime;

    GameObject _virtualCamera;
    Rigidbody _rb;
    Animator _anime;
    Quaternion _targetQuaternion; // 回転制御用

    #region variable

    private float _horizontalInput = 0;
    private float _verticalInput = 0;
    private float _inputSpeed = default;
    private float _rotationSpeedCount = 0;
    private bool _isMouseOn = false; // マウスの表示切替のbool
    private bool _isTerminalOpen = false; // パッド切替のbool

    #endregion

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _virtualCamera = GameObject.FindGameObjectWithTag("Virtual Camera");
        _anime = this.gameObject.GetComponent<Animator>();
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
        TerminalOpen();

        // 攻撃処理
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
        if (!_isTerminalOpen)
        {
            _horizontalInput = Input.GetAxisRaw("Horizontal");
            _verticalInput = Input.GetAxisRaw("Vertical");
        }

        _rotationSpeedCount = _rotationSpeed * Time.deltaTime;

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

            _inputSpeed = Mathf.Max(Mathf.Abs(_horizontalInput), Mathf.Abs(_verticalInput));
        }
        else
        {
            if (_inputSpeed > 0)
            {
                _inputSpeed -= Time.deltaTime;
            }
        }

        _anime.SetFloat("Speed", _inputSpeed); // 移動アニメーションの再生
        transform.rotation = Quaternion.RotateTowards(transform.rotation, _targetQuaternion, _rotationSpeedCount); // 回転速度の制限
    }

    /// <summary>
    /// 端末画面の表示切替
    /// </summary>
    private void TerminalOpen()
    {
        // Eキーを押すと端末画面切替
        if (Input.GetKeyDown(KeyCode.E))
        {
            // アクティブ状態の切替
            _terminalCamera.enabled = !_terminalCamera.enabled; // カメラ
            _mainCanvas.enabled = !_mainCanvas.enabled; // メインキャンバス
            _terminalCanvas.enabled = !_terminalCanvas.enabled; // 端末キャンバス
            _virtualCamera.SetActive(!_virtualCamera.activeSelf); // カメラのコントロール
            Time.timeScale = Time.timeScale == 1  ? Time.timeScale = _timeScale : Time.timeScale = 1; // TimeScale
            _isMouseOn = !_isMouseOn; // マウスの表示
            MouseDisplay();
            _isTerminalOpen = !_isTerminalOpen; // 端末画面切替のbool

            _horizontalInput = 0;
            _verticalInput = 0;
        }
    }

    /// <summary>
    /// マウスの表示処理
    /// </summary>
    private void MouseDisplay()
    {
        //Pキーを押すとマウスの表示切替
        if (Input.GetKeyDown(KeyCode.P))
        {
            _isMouseOn = !_isMouseOn;
            //tamina
            //vi camera
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
    /// 攻撃処理
    /// </summary>
    private void AttackCon()
    {
        // 端末画面ではない状態で左クリックを押すと
        if (!_isTerminalOpen && Input.GetMouseButtonDown(0))
        {
            _attackAnime.SetTrigger("Attack");
        }
    }
}
