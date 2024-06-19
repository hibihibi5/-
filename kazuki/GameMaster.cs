using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameMaster : MonoBehaviour
{
    [Header("変数の指定")]
    [Tooltip("時間制限")]
    [SerializeField] float _timeRimit = default;
    [Tooltip("時間制限のテキスト")]
    [SerializeField] Text _timeRimitText = default;
    [Tooltip("端末画面での時間の速度")]
    [SerializeField, Range(0, 1)] float _terminalTime = default;

    [Header("コンポーネント取得")]
    [Tooltip("")]

    private　const float TIME_COUNT = 1; // 等倍速の時間
    private GameObject _virtualCamera = default;
    private bool _isGame = true; // ゲームが進行しているかどうか
    private bool _isTerminalOpen = false; // 端末が起動しているかどうか

    // public------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// 時間を基本速度にする
    /// </summary>
    public void NormalTime()
    {
        if (_isGame)
        {
            if (!_isTerminalOpen) // 端末状態では無い
            {
                Time.timeScale = TIME_COUNT;
            }
            else　// 端末状態
            {
                TerminalTime();
            }
        }
    }

    /// <summary>
    /// 時間を停止する
    /// </summary>
    public void StopTime()
    {
        if (_isGame)
        {
            Time.timeScale = 0;
        }
    }

    /// <summary>
    /// 時間を端末画面での速度にする
    /// </summary>
    public void TerminalTime()
    {
        if (_isGame)
        {
            Time.timeScale = _terminalTime;
        }
    }

    /// <summary>
    /// 端末状態のbool取得
    /// </summary>
    /// <param name="terminalBool"></param>
    public void GetTerminalOpen(bool terminalBool)
    {
        _isTerminalOpen = terminalBool;
    }

    /// <summary>
    /// ゲームの実行状態を返す
    /// </summary>
    /// <returns></returns>
    public bool GetIsGame()
    {
        return _isGame;
    }

    // private--------------------------------------------------------------------------------------------------------------

    private void Awake()
    {
        _virtualCamera = GameObject.FindGameObjectWithTag("Virtual Camera");
    }

    private void Start()
    {
        // 初期化処理
        _timeRimitText.text = _timeRimit.ToString("F0"); // キャンバスに時間制限の初期値を入れる
    }

    private void FixedUpdate()
    {
        TimeCount();
    }

    /// <summary>
    /// 時間制限の処理
    /// </summary>
    private void TimeCount()
    {
        if (_isGame && (_timeRimit > 0))
        {
            _timeRimit -= Time.deltaTime;
            _timeRimitText.text = _timeRimit.ToString("F0");
        }
        else
        {
            StopTime();
            _isGame = false; // ゲームの終了フラグ
            _timeRimitText.text = "0";
            _virtualCamera.SetActive(false);
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }
}
