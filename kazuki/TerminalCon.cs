using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;
using static WallButtonClass;

public class TerminalCon : MonoBehaviour
{
    [Header("variable")]
    [Tooltip ("コストの貯まる速さ")]
    [SerializeField] float _PointChargeSpeed = 1;
    [Tooltip ("コストの最大値")]
    [SerializeField] float _maxPoint = 999;
    [Tooltip("壁を動かすコスト")]
    [SerializeField] int _wallMoveCost = 0;
    [Tooltip("ボタンのクールタイム")]
    [SerializeField] float _coolTime = 0;
    [Tooltip("ねばねばを置くコスト")]
    [SerializeField] int _nebanebaCost = 0;
    [Tooltip("ねばねばオブジェクトの高さ")]
    [SerializeField] float _nebanebaObjHeight = 0;

    [Header("コンポーネント取得")]
    [Tooltip("壁を動かすボタンs")]
    [SerializeField] ButtonClass[] _buttonClass = new ButtonClass[4];
    [Tooltip("メイン画面のコスト表示テキスト")]
    [SerializeField] Text _mainPointText = default;
    [Tooltip("端末のコスト表示テキスト")]
    [SerializeField] Text _terminalPointText = default;
    [Tooltip("端末カメラ")]
    [SerializeField] Camera _terminalCamera = default;
    [Tooltip("ねばねば選択ボタン")]
    [SerializeField] Button _nebanebaButton;
    [Tooltip("ねばねばオブジェクトの取得")]
    [SerializeField] GameObject[] _nebanebaObjs = new GameObject[1];

    private Vector3 currentPosition = Vector3.zero;
    private int _nebanebaListCount = 0;
    private float _cost = 0; // 現在のコスト
    private float _coolTimeCount = 0; // 現在のクールタイム
    private bool _isButtonClick = false; // ボタン処理の判定
    private bool _isNebanebaClick = false;

    // public---------------------------------------------------------------------------------------



    // private--------------------------------------------------------------------------------------

    private void Start()
    {
        foreach (ButtonClass buttonClass in _buttonClass)
        {
            buttonClass.button.onClick.AddListener(() => WallButton(buttonClass));
        }

        _nebanebaButton.onClick.AddListener(() => NebanebaClick());
    }

    private void Update()
    {
        NebanebaCon();
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

    /// <summary>
    /// ねばねばの選択処理
    /// </summary>
    private void NebanebaClick()
    {
        if (_cost >= _nebanebaCost)
        {
            if (!_isNebanebaClick)
            {
                _isNebanebaClick = true;
            }
            else
            {
                _isNebanebaClick = false;
            }
        }
    }

    /// <summary>
    /// ねばねばの配置処理
    /// </summary>
    private void NebanebaCon()
    {
        if (_isNebanebaClick)
        {
            var ray = _terminalCamera.ScreenPointToRay(Input.mousePosition);
            var raycastHitList = Physics.RaycastAll(ray).ToList();
            if (raycastHitList.Any())
            {
                var distance = Vector3.Distance(_terminalCamera.transform.position, raycastHitList.First().point);
                var mousePosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, distance);

                currentPosition = _terminalCamera.ScreenToWorldPoint(mousePosition);
                currentPosition.y = _nebanebaObjHeight;
            }

            _nebanebaObjs[_nebanebaListCount].GetComponent<MeshCollider>().enabled = false;
            _nebanebaObjs[_nebanebaListCount].transform.position = currentPosition;

            if (Input.GetMouseButtonDown(1))
            {
                _cost -= _nebanebaCost;
                _isNebanebaClick = false;

                _nebanebaObjs[_nebanebaListCount].GetComponent<NebanebaCon>().enabled = true;
                _nebanebaObjs[_nebanebaListCount].GetComponent<MeshCollider>().enabled = true;
                _nebanebaObjs[_nebanebaListCount].transform.position = currentPosition;

                _nebanebaListCount++;
                if (_nebanebaListCount >= _nebanebaObjs.Length)
                {
                    _nebanebaListCount = 0;
                }
            }

            if (Input.GetMouseButtonDown(0))
            {
                _isNebanebaClick = false;
                _nebanebaObjs[_nebanebaListCount].GetComponent<MeshCollider>().enabled = true;
                _nebanebaObjs[_nebanebaListCount].transform.position = Vector3.forward * 100;
            }
        }
    }
}
