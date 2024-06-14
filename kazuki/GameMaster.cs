using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameMaster : MonoBehaviour
{
    [Header("時間制限")]
    [SerializeField] float _timeRimit = default;
    [Header("時間制限のテキスト")]
    [SerializeField] Text _timeRimitText = default;

    private void Start()
    {
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
        if (_timeRimit > 0)
        {
            _timeRimit -= Time.deltaTime;
            _timeRimitText.text = _timeRimit.ToString("F0");
            print("i");
        }
        else
        {
            _timeRimitText.text = "0";
            print("時間切れ");
            Time.timeScale = 0;
        }
    }
}
