using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;
using UnityEngine.EventSystems;
using static WallButtonClass;

public class TerminalCon : MonoBehaviour
{
    [Header("変数の指定")]
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
    [Tooltip("爆弾を置くコスト")]
    [SerializeField] int _SetBombCost = 0;
    [Tooltip("停電コスト")]
    [SerializeField] int _PowerOutageCost = 0;
    [Tooltip("停電のクールタイム")]
    [SerializeField] float _coolTimeOfPowerOut = 0;

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
    [Tooltip("爆弾の強制起爆ボタン")]
    [SerializeField] Button _explosionButton = default;
    [Tooltip("停電ボタン")]
    [SerializeField] Button _powerOutButton = default;
    [Tooltip("停電用親取得")]
    [SerializeField] GameObject power;
    [Tooltip("効果音用取得")]
    [SerializeField] private GameObject seObj;
    [Tooltip("説明欄取得")]
    [SerializeField] GameObject explan;

    private Vector3 currentPosition = Vector3.zero;
    private int _nebanebaListCount = 0;
    private float _cost = 0; // 現在のコスト
    private float _coolTimeCount = 0; // 現在のクールタイム
    private float _coolTimeCountOfPower = 0; // 現在の停電クールタイム
    private bool _isButtonClick = false; // ボタン処理の判定
    private bool _isNebanebaClick = false;

    // public---------------------------------------------------------------------------------------

    /// <summary>
    /// 爆弾を置けるかどうかの処理
    /// </summary>
    /// <returns></returns>
    public bool GetBombCost()
    {
        if (_cost >= _SetBombCost)
        {
            _cost -= _SetBombCost;
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// 外部からのネバ設置状態の解除
    /// </summary>
    public void OutCancelNebanebaSet()
    {
        CancelNebanebaSet();
    }

    // private--------------------------------------------------------------------------------------

    private void Awake()
    {
        //EventTriggerコンポーネントを取得
        EventTrigger eventTrigger = GameObject.FindGameObjectWithTag("EventTrigger").GetComponent<EventTrigger>();

        //イベントの設定に入る
        EventTrigger.Entry entry = new EventTrigger.Entry();

        //PointerDown(押した瞬間に実行する)イベントタイプを設定
        entry.eventID = EventTriggerType.PointerDown;

        //関数を設定
        entry.callback.AddListener((x) =>
        {
            NebaSet();
        });

        //イベントの設定をEventTriggerに反映
        eventTrigger.triggers.Add(entry);
    }

    private void Start()
    {
        foreach (ButtonClass buttonClass in _buttonClass)
        {
            buttonClass.button.onClick.AddListener(() => WallButton(buttonClass));
        }

        _nebanebaButton.onClick.AddListener(() => NebanebaClick());

        _explosionButton.onClick.AddListener(() => BombExplosion());

        _powerOutButton.onClick.AddListener(() => PowerOutage());
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
        _mainPointText.text = _cost.ToString("F1"); // メインキャンバス
        _terminalPointText.text = _cost.ToString("F1"); // 端末キャンバス
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

        if (_coolTimeCountOfPower > 0)
        {
            _coolTimeCountOfPower -= Time.deltaTime;
        }
        else
        {
            _powerOutButton.interactable = true;
        }
    }

    /// <summary>
    /// 壁用ボタンの処理
    /// </summary>
    /// <param name="buttonClass"></param>
    private void WallButton(ButtonClass buttonClass)
    {
        CancelNebanebaSet();

        if (!_isButtonClick && (_cost >= _wallMoveCost))
        {
            _coolTimeCount = _coolTime;
            _isButtonClick = true;
            seObj.GetComponent<SEScript>().Wall();
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
        else if(_cost < _wallMoveCost)
        {
            explan.GetComponent<ExplanationScri>().ShutterTarinu();
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
                seObj.GetComponent<SEScript>().Cleck();
                _isNebanebaClick = true;
                _nebanebaButton.interactable = false; // ボタンを非アクティブ状態に
            }
        }
        else
        {
            explan.GetComponent<ExplanationScri>().NebanebaTarinu();
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

            _nebanebaObjs[_nebanebaListCount].GetComponent<SphereCollider>().enabled = false;
            _nebanebaObjs[_nebanebaListCount].transform.position = currentPosition;

            // 右クリックしたときに設置状態を解除
            if (Input.GetMouseButtonDown(1))
            {
                CancelNebanebaSet();
            }
        }
    }

    /// <summary>
    /// ネバ設置状態の解除
    /// </summary>
    private void CancelNebanebaSet()
    {
        if (_isNebanebaClick)
        {
            _isNebanebaClick = false;
            _nebanebaButton.interactable = true; // ボタンをアクティブ状態に
            _nebanebaObjs[_nebanebaListCount].GetComponent<SphereCollider>().enabled = true; // 当たり判定をオン
            _nebanebaObjs[_nebanebaListCount].transform.position = Vector3.zero;
        }
    }

    /// <summary>
    /// ねばねばの設置処理
    /// </summary>
    private void NebaSet()
    {
        if (_isNebanebaClick && Input.GetMouseButtonDown(0))
        {
            _isNebanebaClick = false;
            _nebanebaButton.interactable = true; // ボタンをアクティブ状態に
            _cost -= _nebanebaCost; // コストの消費

            _nebanebaObjs[_nebanebaListCount].GetComponent<NebanebaCon>().enabled = true; // 消滅処理の開始　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　
            _nebanebaObjs[_nebanebaListCount].GetComponent<SphereCollider>().enabled = true; // 当たり判定をオフ
            _nebanebaObjs[_nebanebaListCount].transform.position = currentPosition;

            _nebanebaListCount++;
            if (_nebanebaListCount >= _nebanebaObjs.Length)
            {
                _nebanebaListCount = 0;
            }
        }
    }

    /// <summary>
    /// 全ての爆弾の強制起爆処理
    /// </summary>
    private void BombExplosion()
    {
        CancelNebanebaSet();

        if (GameObject.FindGameObjectsWithTag("Bomb").Length >= 1)
        {
            seObj.GetComponent<SEScript>().Bomb();
        }
        foreach (GameObject bombCon in GameObject.FindGameObjectsWithTag("Bomb"))
        {
            bombCon.GetComponent<BombCon>().CompulsionExplosion();
        }
    }
    /// <summary>
    /// 停電処理
    /// </summary>
    private void PowerOutage()
    {
        CancelNebanebaSet();
        if (_cost >= _PowerOutageCost)
        {
            power.GetComponent<PowerOut>().PowerOutage();
            _powerOutButton.interactable = false;
            _coolTimeCountOfPower = _coolTimeOfPowerOut;
            _cost -= _PowerOutageCost;
            seObj.GetComponent<SEScript>().PowerOut();
        }
        else
        {
            explan.GetComponent<ExplanationScri>().PowerOutTarinu();
        }
    }
}
