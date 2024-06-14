using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseController1 : MonoBehaviour
{
    [SerializeField] private Canvas _pauseScreen;

    const int TIME_COUNT = 1;

    private void Start()
    {
        // 初期化処理
        _pauseScreen.enabled = false; // ポーズキャンバスの非表示
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
            // TimeScaleが1のとき0に0のとき1に
            Time.timeScale = Time.timeScale == 1 ? Time.timeScale = 0 : Time.timeScale = TIME_COUNT;

            // キャンバスの切り替え
            _pauseScreen.enabled = !_pauseScreen.enabled;
        }
    }
}
