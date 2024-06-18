using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using static WallButtonClass;

public class TerminalCon : MonoBehaviour
{
    [SerializeField] ButtonClass[] _buttonClass = new ButtonClass[4];

    [Header("variable")]
    [Tooltip ("コストの貯まる速さ")]
    [SerializeField] float _PointChargeSpeed = 1;
    [Tooltip ("コストの最大値")]
    [SerializeField] float _maxPoint = 100;
    [Tooltip("壁を動かすコスト")]
    [SerializeField] int _wallMoveCost = 0;
    [Tooltip("ボタンのクールタイム")]
    [SerializeField] float _coolTime = 0;

    [Header("コンポーネント取得")]
    [Tooltip("メイン画面のコスト表示テキスト")]
    [SerializeField] Text _mainPointText = default;
    [Tooltip("端末のコスト表示テキスト")]
    [SerializeField] Text _terminalPointText = default;

    private float _cost = 0; // 現在のコスト
    private float _coolTimeCount = 0; // 現在のクールタイム
    private bool _isButtonClick = false; // ボタン処理の判定

    // public---------------------------------------------------------------------------------------



    // private--------------------------------------------------------------------------------------

    private void Start()
    {
        foreach (ButtonClass buttonClass in _buttonClass)
        {
            buttonClass.button.onClick.AddListener(() => WallButton(buttonClass));
        }
    }

    private void FixedUpdate()
    {
        CostDisplayUpdate();

        CoolTimeCount();
    }

    /// <summary>
    /// コストの更新処理
    /// </summary>
    private void CostDisplayUpdate()
    {
        _cost += _PointChargeSpeed * Time.deltaTime; // 時間経過で設定した速度でコストが貯まる
        _cost = Mathf.Clamp(_cost, 0, _maxPoint); // 最大値にクランプ
        _mainPointText.text = _cost.ToString("F2"); // メインキャンバス
        _terminalPointText.text = _cost.ToString("F2"); // 端末キャンバス
    }

    /// <summary>
    /// クールタイムの処理
    /// </summary>
    private void CoolTimeCount()
    {
        if (_coolTimeCount > 0)
        {
            _coolTimeCount -= Time.deltaTime;
        }
        else
        {
            _isButtonClick = false;

            foreach (ButtonClass button in _buttonClass)
            {
                // ボタンの有効化
                button.button.interactable = true;
            }
        }
    }

    /// <summary>
    /// 壁用ボタンの処理
    /// </summary>
    /// <param name="buttonClass"></param>
    private void WallButton(ButtonClass buttonClass)
    {
        if (!_isButtonClick && (_cost >= _wallMoveCost))
        {
            _coolTimeCount = _coolTime;
            _isButtonClick = true;

            _cost -= _wallMoveCost;

            bool isAnimeBool = buttonClass.animator[0].GetBool("WallBool");

            foreach (ButtonClass button in _buttonClass)
            {
                // ボタンの無効化
                button.button.interactable = false;

                // 全ての扉を閉める
                foreach (Animator anime in button.animator)
                {
                    anime.SetBool("WallBool", false);
                }
            }

            foreach (Animator animator in buttonClass.animator)
            {
                animator.SetBool("WallBool", !isAnimeBool);
            }
        }
    }
}
