using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class PauseCon : MonoBehaviour
{
    [Header("コンポーネント取得")]
    [Tooltip("ゲームマスターオブジェクト")]
    [SerializeField] GameMaster _gameMaster;
    [Tooltip("端末操作スクリプト")]
    [SerializeField] TerminalCon _terminalCon;
    [Tooltip("ポーズ画面のキャンバス")]
    [SerializeField] private Canvas _pauseCanvas;
    [Tooltip("カメラ制御オブジェクト")]
    [SerializeField] private CinemachineVirtualCamera _virtualCamera = default;

    private bool _isPause = false;

    private void Start()
    {
        // 初期化処理
        _pauseCanvas.enabled = false; // ポーズキャンバスの非表示
    }

    private void Update()
    {
        // ポーズ画面の表示
        PauseGame();
    }

    /// <summary>
    /// ポーズ画面の表示
    /// </summary>
    private void PauseGame()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            bool isGame = _gameMaster.GetIsGame();

            if (isGame && !_isPause) // アクティブ状態
            {
                _isPause = true;
                _terminalCon.OutCancelNebanebaSet();
                _gameMaster.GetPouseBool(_isPause);
                _gameMaster.StopTime(); // 時間の停止
                _pauseCanvas.enabled = true;
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                _virtualCamera.enabled = false;
            }
            else if (isGame) // 非アクティブ状態
            {
                _isPause = false;
                _gameMaster.GetPouseBool(_isPause);
                _gameMaster.PouseEscape();
                _pauseCanvas.enabled = false;

                bool isTerminal = _gameMaster.SetTerminalOpen();
                if (!isTerminal)
                {
                    Cursor.visible = false;
                    Cursor.lockState = CursorLockMode.Locked;
                    _virtualCamera.enabled = true;
                }
            }
        }
    }
}
