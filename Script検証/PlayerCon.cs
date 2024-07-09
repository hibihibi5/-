using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.InputSystem;

public class PlayerCon : MonoBehaviour
{
    #region setVariable

    [Header("変数の指定")]
    [Tooltip("プレイヤーの通常時の移動速度")]
    [SerializeField] float _playerNormalMoveSpeed = 15f;
    //[Tooltip("プレイヤーのダッシュ時の移動速度")]
    //[SerializeField] float _playerDashSpeed = default;
    [Tooltip("プレイヤーの移動の変化速度")]
    [SerializeField] float _playerMoveChegeSpeed = default;
    [Tooltip("重力の設定")]
    [SerializeField] Vector3 _playerGravity = new Vector3(0, -200f, 0);
    [Tooltip("プレイヤーの回転速度")]
    [SerializeField] float _rotationSpeed = 900f;
    [Tooltip("攻撃時のプレイヤーの移動速度")]
    [SerializeField, Range(0, 1)] float _setAttackMoveSpeed = 0.5f;
    [Tooltip("ねばねば床での速度")]
    [SerializeField, Range(0, 1)] float _nebanebaSpeed = 0.4f;
    [Tooltip("移動アニメーションの変化の速度")]
    [SerializeField] float _moveAnimeChegeSpeed = 1;
    [Tooltip("爆弾の設置に掛かる時間")]
    [SerializeField] float _BombSetTime = default;
    [Tooltip("爆弾をどこに生成するか")]
    [SerializeField] float _bombSetPoint = default;

    #endregion

    #region setComponent

    [Header("コンポーネント取得")]
    [SerializeField] GameMaster _gameMaster;
    [SerializeField] Camera _terminalCamera;
    [SerializeField] Canvas _terminalCanvas;
    [SerializeField] Canvas _mainCanvas;
    [Tooltip("カメラ制御オブジェクト")]
    [SerializeField] private CinemachineVirtualCamera _virtualCamera = default;
    [Tooltip("インプットシステムコンポーネント取得")]
    [SerializeField] PlayerInput _playerInput = default;
    [SerializeField] private GameObject seObj;

    #endregion

    #region component

    Rigidbody _rb;
    Animator _playerAnime;
    Quaternion _targetQuaternion; // 回転制御用
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
    private float _playerMoveSpeedNow = default; // 現在のプレイヤーの移動速度
    private float _inputDeshSpeed = 1;
    private bool _isMouseOn = false; // マウスの表示切替のbool
    private bool _isTerminalOpen = false; // 端末切替のbool
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

        // 爆弾の設置開始処理
        BombSetStart();
    }

    private void FixedUpdate()
    {
        _rb.AddForce(_playerGravity, ForceMode.Acceleration); // 重力処理

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
    /// プレイヤーの移動処理
    /// </summary>
    private void PlayerMove()
    {
        if (!_isTerminalOpen && !_isBombSet && !_gameMaster.SetPouseBool())
        {
            var inputMoveAxis = _moveInput.ReadValue<Vector2>();
            _horizontalInput = inputMoveAxis.x;
            _verticalInput = inputMoveAxis.y;

            //if (!Input.GetKey(KeyCode.LeftShift)) // ノーマル
            //{
            //    _isPlayerDesh = false;
            //}
            //else // ダッシュ
            //{
            //    _isPlayerDesh = true;
            //}
        }

        _rotationSpeedCount = _rotationSpeed * Time.deltaTime;

        // 入力しているとき
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

            // カメラの方向から、X-Z平面の単位ベクトルを取得
            Vector3 cameraForward = Vector3.Scale(Camera.main.transform.forward, new Vector3(1, 0, 1)).normalized;

            // 方向キーの入力値とカメラの向きから、移動方向を決定
            _moveForward = cameraForward * _verticalInput + Camera.main.transform.right * _horizontalInput;
            _moveForward = _moveForward.normalized;

            // 移動方向にスピードを掛ける。
            _rb.velocity = _moveForward * _playerNormalMoveSpeed * _attackMoveSpeed * _externalSpeed;

            // キャラクターの向きを進行方向に
            if (_moveForward != Vector3.zero)
            {
                _targetQuaternion = Quaternion.LookRotation(_moveForward);
            }

            //// アニメーションの遷移処理
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

            //if (_inputSpeed < 0.4f) // ノーマル
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

        _playerAnime.SetFloat("Speed", _inputSpeed/* * _inputDeshSpeed*/); // 移動アニメーションの再生
        _rb.MoveRotation(Quaternion.RotateTowards(transform.rotation, _targetQuaternion, _rotationSpeedCount)); // 回転速度の制限
    }

    /// <summary>
    /// 端末画面の表示切替
    /// </summary>
    private void TerminalOpen()
    {
        // Eキーを押すと端末画面切替
        if (_terminalInput.WasPressedThisFrame() && !_gameMaster.SetPouseBool() && _gameMaster.GetIsGame())
        {
            if (!_isTerminalOpen && !_isBombSet) // アクティブ状態
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
                _virtualCamera.enabled = false; // カメラのコントロール
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
                _virtualCamera.enabled=true; // カメラのコントロール
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
    /// 攻撃の終了処理
    /// </summary>
    public void AttackEnd()
    {
        _playerAnime.SetBool("attack", false);
        _isAttack = false;

        _attackMoveSpeed = ONE;
    }

    /// <summary>
    /// 爆弾の設置開始処理
    /// </summary>
    private void BombSetStart()
    {
        // 端末画面ではない状態で左クリックを押すと
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
    /// 爆弾の設置処理
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
