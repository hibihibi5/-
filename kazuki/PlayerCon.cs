using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCon : MonoBehaviour
{
    [Header("変数の指定")]
    [Tooltip("プレイヤーの移動速度")]
    [SerializeField] float _playerMoveSpeed = 0f;
    [Tooltip("重力の設定")]
    [SerializeField] Vector3 _playerGravity = new Vector3(0, -9.81f, 0);
    [Tooltip("端末画面のTimeScale")]
    [SerializeField, Range(0, 1)] float _timeScale = default;
    [Tooltip("プレイヤーの回転速度")]
    [SerializeField] float _rotationSpeed = 900f;
    [Tooltip("攻撃時のプレイヤーの移動速度")]
    [SerializeField, Range(0, 1)] float _setAttackMoveSpeed = 0.4f;

    [Header("コンポーネント取得")]
    [SerializeField] GameMaster _gameMaster;
    [SerializeField] Camera _terminalCamera;
    [SerializeField] Canvas _terminalCanvas;
    [SerializeField] Canvas _mainCanvas;

    GameObject _virtualCamera;
    Rigidbody _rb;
    Animator _PlayerAnime;
    Quaternion _targetQuaternion; // 回転制御用

    #region variable

    private Vector3 _moveForward = default;
    private float _horizontalInput = 0;
    private float _verticalInput = 0;
    private float _inputSpeed = default;
    private float _rotationSpeedCount = 0;
    private float _attackMoveSpeed = 1;
    private bool _isMouseOn = false; // マウスの表示切替のbool
    private bool _isTerminalOpen = false; // 端末切替のbool

    #endregion

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _virtualCamera = GameObject.FindGameObjectWithTag("Virtual Camera");
        _PlayerAnime = this.gameObject.GetComponent<Animator>();
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
        //MouseDisplay();

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
            _moveForward = cameraForward * _verticalInput + Camera.main.transform.right * _horizontalInput;
            _moveForward = _moveForward.normalized;

            // 移動方向にスピードを掛ける。
            _rb.velocity = _moveForward * _playerMoveSpeed * _attackMoveSpeed;

            // キャラクターの向きを進行方向に
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

        _PlayerAnime.SetFloat("Speed", _inputSpeed); // 移動アニメーションの再生
        transform.rotation = Quaternion.RotateTowards(transform.rotation, _targetQuaternion, _rotationSpeedCount); // 回転速度の制限
    }

    /// <summary>
    /// 端末画面の表示切替
    /// </summary>
    private void TerminalOpen()
    {
        // Eキーを押すと端末画面切替
        if (Input.GetKeyDown(KeyCode.E) && (Time.timeScale != 0))
        {
            if (!_isTerminalOpen) // アクティブ状態
            {
                _isTerminalOpen = true;
                _gameMaster.GetTerminalOpen(_isTerminalOpen); // 端末状態の送信
                _gameMaster.TerminalTime(); // 時間の速度低下

                // on
                _terminalCamera.enabled = true; // カメラ
                _terminalCanvas.enabled = true; // 端末キャンバス
                _isMouseOn = true;
                MouseDisplay();
                // off
                _mainCanvas.enabled = false; // メインキャンバス
                _virtualCamera.SetActive(false); // カメラのコントロール
                _horizontalInput = 0;
                _verticalInput = 0;
            }
            else // 非アクティブ状態
            {
                _isTerminalOpen = false;
                _gameMaster.GetTerminalOpen(_isTerminalOpen); // 端末状態の送信
                _gameMaster.NormalTime(); // 時間の基本状態に

                // on
                _mainCanvas.enabled = true; // メインキャンバス
                _virtualCamera.SetActive(true); // カメラのコントロール
                // off
                _terminalCamera.enabled = false; // カメラ
                _terminalCanvas.enabled = false; // 端末キャンバス
                _isMouseOn = false;
                MouseDisplay();
            }
        }
    }

    /// <summary>
    /// マウスの表示処理
    /// </summary>
    private void MouseDisplay()
    {
        ////Pキーを押すとマウスの表示切替
        //if (Input.GetKeyDown(KeyCode.P))
        //{
        //    _isMouseOn = !_isMouseOn;
        //}

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
            _isTerminalOpen = true;
            _PlayerAnime.SetBool("attack", true);

            _attackMoveSpeed = _setAttackMoveSpeed;
        }
    }

    /// <summary>
    /// 攻撃の終了処理
    /// </summary>
    public void AttackEnd()
    {
        _PlayerAnime.SetBool("attack", false);
        _isTerminalOpen = false;

        _attackMoveSpeed = 1;
    }
}
