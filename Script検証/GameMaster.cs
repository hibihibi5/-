using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;

public class GameMaster : MonoBehaviour
{
    [Header("変数の指定")]
    [Tooltip("時間制限")]
    [SerializeField] float _timeRimit = default;
    [Tooltip("端末画面での時間の速度")]
    [SerializeField, Range(0, 1)] float _terminalTime = default;
    [Tooltip("ゲーム終了時の時間の速度")]
    [SerializeField, Range(0, 1)] float _endGameTime = default;

    [Header("コンポーネント取得")]
    [Tooltip("メイン画面の時間制限のテキスト")]
    [SerializeField] Text _mainTimeRimitText = default;
    [Tooltip("端末画面の時間制限テキスト")]
    [SerializeField] Text _tarminalTimeRimitText = default;
    //[Tooltip("ゲーム説明のキャンバス")]
    //[SerializeField] Canvas _explanationCanvas = default;
    [Tooltip("カメラ制御オブジェクト")]
    [SerializeField] private CinemachineVirtualCamera _virtualCamera = default;

    private　const float NORMAL_TIME = 1; // 等倍速の時間
    private bool _isGame = true; // ゲームが進行しているかどうか
    private bool _isTerminalOpen = false; // 端末が起動しているかどうか
    private bool _isPause = false; // ポーズ状態かどうか

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
                Time.timeScale = NORMAL_TIME;
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
    /// ポーズ状態を解除
    /// </summary>
    public void PouseEscape()
    {
        if (!_isTerminalOpen) // 通常
        {
            NormalTime();
        }
        else // 端末
        {
            TerminalTime();
        }
    }

    /// <summary>
    /// 端末の状態取得
    /// </summary>
    /// <param name="terminalBool"></param>
    public void GetTerminalOpen(bool terminalBool)
    {
        _isTerminalOpen = terminalBool;
    }

    /// <summary>
    /// 端末の状態を返す
    /// </summary>
    /// <returns></returns>
    public bool SetTerminalOpen()
    {
        return _isTerminalOpen;
    }

    /// <summary>
    /// ポーズの状態取得
    /// </summary>
    /// <param name="isPouse"></param>
    public void GetPouseBool(bool isPouse)
    {
        _isPause = isPouse;
    }

    /// <summary>
    /// ポーズの状態を返す
    /// </summary>
    /// <returns></returns>
    public bool SetPouseBool()
    {
        return _isPause;
    }

    /// <summary>
    /// ゲームの実行状態を返す
    /// </summary>
    /// <returns></returns>
    public bool GetIsGame()
    {
        return _isGame;
    }

    /// <summary>
    /// ゲームの実行状態を取得
    /// </summary>
    public void SetGameOver()
    {
        _isGame = false;
    }

    // private--------------------------------------------------------------------------------------------------------------

    private void Start()
    {
        // 初期化処理
        Time.timeScale = NORMAL_TIME;
        _mainTimeRimitText.text = _timeRimit.ToString("F0"); // キャンバスに時間制限の初期値を入れる
        _tarminalTimeRimitText.text = _timeRimit.ToString("F0"); // キャンバスに時間制限の初期値を入れる

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
        if (_isGame && !_isPause && (_timeRimit > 0))
        {
            _timeRimit -= Time.deltaTime;
            _mainTimeRimitText.text = _timeRimit.ToString("F0");
            _tarminalTimeRimitText.text = _timeRimit.ToString("F0");
        }
        else if (!_isPause)
        {
            Time.timeScale = _endGameTime;
            _isGame = false; // ゲームの終了フラグ
            _mainTimeRimitText.text = "0";
            _tarminalTimeRimitText.text = "0";
            _virtualCamera.enabled = false;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }
}
