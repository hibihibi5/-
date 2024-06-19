using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseCon : MonoBehaviour
{
    [Header("コンポーネント取得")]
    [Tooltip("ゲームマスターオブジェクト")]
    [SerializeField] GameMaster _gameMaster;
    [Tooltip("ポーズ画面のキャンバス")]
    [SerializeField] private Canvas _pauseCanvas;

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
                _gameMaster.StopTime(); // 時間の停止
                _pauseCanvas.enabled = true;
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }
            else if (isGame) // 非アクティブ状態
            {
                _isPause = false;
                _gameMaster.NormalTime(); // 時間の再開
                _pauseCanvas.enabled = false;
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }
        }
    }
}
